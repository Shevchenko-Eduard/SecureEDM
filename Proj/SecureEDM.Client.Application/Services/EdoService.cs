using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using SecureEDM.Client.Application.Interfaces;
using SecureEDM.Client.Application.Models;
using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Application.Services;

/// <summary>
/// Логика учебного клиента ЭДО: документы, ключи, подписи, журнал и проверка пакетов.
/// Приватные ключи живут только в памяти процесса и никогда не сохраняются на диск.
/// </summary>
public sealed class EdoService(IRegistryStore store, IClientCrypto crypto)
{
    private static readonly JsonSerializerOptions CompactJson = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    private static readonly JsonSerializerOptions PackageJson = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly IRegistryStore _store = store;
    private readonly IClientCrypto _crypto = crypto;
    private readonly Dictionary<string, byte[]> _privateKeys = new(StringComparer.Ordinal);

    private EdoPersistedState _state = new();
    private byte[] _sessionSecret = [];
    private bool _initialized;

    public IReadOnlyList<EdoUser> Users => EdoUsers.All;
    public IReadOnlyList<EdoDocument> Documents => _state.Documents;
    public IReadOnlyList<EdoKey> Keys => _state.Keys;
    public IReadOnlyList<EdoSignature> Signatures => _state.Signatures;
    public IReadOnlyList<EdoHashRecord> Hashes => _state.Hashes;
    public IReadOnlyList<EdoTrackingFile> TrackingFiles => _state.TrackingFiles;
    public EdoSettings Settings => _state.Settings;
    public IReadOnlyList<EdoLogEntry> Log => _state.Log;

    public string SessionUserId => _state.SessionUserId;
    public string Theme => _state.Theme;
    public EdoUser CurrentUser =>
        EdoUsers.All.FirstOrDefault(u => u.Id == _state.SessionUserId) ?? EdoUsers.All[0];

    public async ValueTask InitializeAsync(CancellationToken token = default)
    {
        if (_initialized) return;

        var loaded = await _store.LoadAsync(token);
        _state = loaded ?? new EdoPersistedState();

        _state.Documents ??= [];
        _state.Keys ??= [];
        _state.Signatures ??= [];
        _state.Hashes ??= [];
        _state.TrackingFiles ??= [];
        _state.Settings ??= new EdoSettings();
        _state.Log ??= [];

        if (string.IsNullOrWhiteSpace(_state.SessionUserId))
            _state.SessionUserId = EdoUsers.All[0].Id;
        if (EdoUsers.All.All(u => u.Id != _state.SessionUserId))
            _state.SessionUserId = EdoUsers.All[0].Id;

        if (string.IsNullOrWhiteSpace(_state.SessionSecretBase64))
            _state.SessionSecretBase64 = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        try
        {
            _sessionSecret = Convert.FromBase64String(_state.SessionSecretBase64);
        }
        catch (FormatException)
        {
            _sessionSecret = RandomNumberGenerator.GetBytes(32);
            _state.SessionSecretBase64 = Convert.ToBase64String(_sessionSecret);
        }

        _initialized = true;
        await _store.SaveAsync(_state, token);
    }

    public bool Can(string action)
    {
        var role = CurrentUser.Role;
        return action switch
        {
            "editDoc" => role is "author" or "admin",
            "sign" => role is "author" or "admin",
            "manageKey" => role is "author" or "admin",
            "manageFiles" => role is "author" or "admin",
            "manageSettings" => role is "author" or "admin",
            "clear" => role == "admin",
            "export" => role == "admin",
            _ => true,
        };
    }

    public EdoDocument? FindDocument(string? id) =>
        id is null ? null : _state.Documents.FirstOrDefault(d => d.Id == id);

    public EdoKey? FindKey(string? id) =>
        id is null ? null : _state.Keys.FirstOrDefault(k => k.Id == id);

    public EdoSignature? FindSignature(string? id) =>
        id is null ? null : _state.Signatures.FirstOrDefault(s => s.Id == id);

    public EdoHashRecord? FindHash(string? id) =>
        id is null ? null : _state.Hashes.FirstOrDefault(h => h.Id == id);

    public EdoTrackingFile? FindTrackingFile(string? id) =>
        id is null ? null : _state.TrackingFiles.FirstOrDefault(f => f.Id == id);

    public bool KeyIsActive(EdoKey key) =>
        key.Status != EdoKeyStatus.Revoked && key.ValidUntil > DateTimeOffset.Now;

    public bool HasPrivateKey(string keyId) => _privateKeys.ContainsKey(keyId);

    public (string Kind, string Text) GetDocumentStatus(EdoDocument document)
    {
        var signatures = _state.Signatures.Where(s => s.DocumentId == document.Id).ToList();
        if (signatures.Count == 0) return ("none", "Не подписан");

        var activeSignatures = signatures.Where(s => s.RevokedAt is null).ToList();
        if (activeSignatures.Count == 0) return ("bad", "Подпись отозвана");

        foreach (var signature in activeSignatures)
        {
            var key = FindKey(signature.KeyId);
            if (key is null || key.Status == EdoKeyStatus.Revoked) continue;
            if (string.Equals(signature.Hash, document.Hash, StringComparison.OrdinalIgnoreCase))
                return ("ok", "Подписан");
        }

        return ("warn", "Изменён после подписания");
    }

    public ValueTask<string> ComputeTextHashAsync(string text, CancellationToken token = default) =>
        _crypto.ComputeSha256Async(text, token);

    public ValueTask<string> GetDocumentHashAsync(EdoDocument document, CancellationToken token = default) =>
        _crypto.ComputeSha256Async(CanonicalDocument(document.Number, document.Title, document.Author, document.Content), token);

    public async Task<EdoDocument> CreateDocumentAsync(
        string title, string author, string content, CancellationToken token = default)
    {
        var number = "Д-" + (_state.Documents.Count + 1).ToString("D3");
        var canonical = CanonicalDocument(number, title, author, content);
        var hash = await _crypto.ComputeSha256Async(canonical, token);
        var document = new EdoDocument
        {
            Id = NewId(),
            Number = number,
            Title = title,
            Author = author,
            Content = content,
            Hash = hash,
            Version = 0,
            CreatedAt = DateTimeOffset.Now,
            UpdatedAt = DateTimeOffset.Now,
        };

        _state.Documents.Add(document);
        _state.Hashes.Add(new EdoHashRecord
        {
            Id = NewId(),
            DocumentId = document.Id,
            Algorithm = EnumDigestAlgorithm.SHA_256,
            HashHex = hash,
            IsPrimary = true,
            CreatedAt = DateTimeOffset.Now,
        });

        await LogEventAsync("Создание", $"Документ {document.Number} «{title}»", token);
        return document;
    }

    public async Task<(EdoDocument Document, bool Changed)> UpdateDocumentAsync(
        string id, string title, string author, string content, CancellationToken token = default)
    {
        var document = FindDocument(id) ?? throw new InvalidOperationException("Документ не найден.");
        var canonical = CanonicalDocument(document.Number, title, author, content);
        var newHash = await _crypto.ComputeSha256Async(canonical, token);
        var changed = !string.Equals(document.Hash, newHash, StringComparison.OrdinalIgnoreCase);

        if (changed)
        {
            foreach (var hashRecord in _state.Hashes.Where(h =>
                         h.DocumentId == document.Id && h.RevokedAt is null))
            {
                hashRecord.RevokedAt = DateTimeOffset.Now;
                hashRecord.RevokedReason = EnumRevokedReason.Outdated;
                hashRecord.IsPrimary = false;
            }
        }

        if (changed)
        {
            document.History ??= [];
            document.History.Add(new EdoDocumentHistory
            {
                Version = document.Version,
                Hash = document.Hash,
                Title = document.Title,
                Author = document.Author,
                Content = document.Content,
                ArchivedAt = DateTimeOffset.Now,
            });
            document.Version += 1;
        }

        document.Title = title;
        document.Author = author;
        document.Content = content;
        document.Hash = newHash;
        document.UpdatedAt = DateTimeOffset.Now;

        if (changed)
        {
            _state.Hashes.Add(new EdoHashRecord
            {
                Id = NewId(),
                DocumentId = document.Id,
                Algorithm = EnumDigestAlgorithm.SHA_256,
                HashHex = newHash,
                IsPrimary = true,
                CreatedAt = DateTimeOffset.Now,
            });
        }

        await LogEventAsync(
            "Изменение",
            $"Документ {document.Number} «{title}» сохранён{(changed ? ", содержимое изменилось" : string.Empty)}",
            token);

        return (document, changed);
    }

    public async Task DeleteDocumentAsync(string id, CancellationToken token = default)
    {
        var document = FindDocument(id) ?? throw new InvalidOperationException("Документ не найден.");

        _state.Documents.Remove(document);
        _state.Signatures.RemoveAll(s => s.DocumentId == id);
        _state.Hashes.RemoveAll(h => h.DocumentId == id);

        await LogEventAsync("Удаление", $"Документ {document.Number} «{document.Title}» удалён", token);
    }

    public async Task<EdoHashRecord> AddHashAsync(
        string documentId, EnumDigestAlgorithm algorithm, CancellationToken token = default)
    {
        var document = FindDocument(documentId)
            ?? throw new InvalidOperationException("Документ не найден.");

        var canonical = CanonicalDocument(document.Number, document.Title, document.Author, document.Content);
        var value = await _crypto.ComputeHashAsync(algorithm, canonical, token);

        var record = new EdoHashRecord
        {
            Id = NewId(),
            DocumentId = document.Id,
            Algorithm = algorithm,
            HashHex = value,
            IsPrimary = !_state.Hashes.Any(h => h.DocumentId == document.Id && h.RevokedAt is null),
            CreatedAt = DateTimeOffset.Now,
        };

        _state.Hashes.Add(record);
        await LogEventAsync(
            "Хеши",
            $"Для документа {document.Number} добавлен {EdoLabels.HashAlgorithm(algorithm)} ({ShortHash(value)})",
            token);

        return record;
    }

    public async Task SetPrimaryHashAsync(string hashId, CancellationToken token = default)
    {
        var hash = FindHash(hashId) ?? throw new InvalidOperationException("Хеш не найден.");
        if (hash.RevokedAt is not null)
            throw new InvalidOperationException("Нельзя сделать основной отозванный хеш.");

        foreach (var item in _state.Hashes.Where(h =>
                     h.DocumentId == hash.DocumentId && h.RevokedAt is null))
            item.IsPrimary = false;

        hash.IsPrimary = true;
        await LogEventAsync("Хеши", $"Основной хеш документа: {EdoLabels.HashAlgorithm(hash.Algorithm)}", token);
    }

    public async Task RevokeHashAsync(
        string hashId, EnumRevokedReason reason, CancellationToken token = default)
    {
        var hash = FindHash(hashId) ?? throw new InvalidOperationException("Хеш не найден.");
        if (hash.RevokedAt is not null) return;

        hash.RevokedAt = DateTimeOffset.Now;
        hash.RevokedReason = reason;
        hash.IsPrimary = false;
        await LogEventAsync("Хеши", $"Хеш {ShortHash(hash.HashHex)} отозван ({EdoLabels.RevokedReason(reason)})", token);
    }

    public async Task DeleteHashAsync(string hashId, CancellationToken token = default)
    {
        var hash = FindHash(hashId) ?? throw new InvalidOperationException("Хеш не найден.");
        _state.Hashes.Remove(hash);
        await LogEventAsync("Хеши", $"Хеш {ShortHash(hash.HashHex)} удалён", token);
    }

    public Task<EdoKey> GenerateKeyAsync(
        string owner, EdoKeyAlgorithm algorithm, int days, CancellationToken token = default) =>
        GenerateKeyAsync(
            owner,
            EdoAlgorithms.ToKeygenAlgorithm(algorithm),
            EdoAlgorithms.KeyLength(algorithm),
            EdoAlgorithms.ToSignedAlgorithm(algorithm),
            days,
            token);

    public async Task<EdoKey> GenerateKeyAsync(
        string owner, EnumKpgAlgorithm algorithm, int keyLength,
        EnumSignedAlgorithm? defaultSignatureAlgorithm, int days,
        CancellationToken token = default)
    {
        days = Math.Clamp(days, 1, 3650);
        if (!EdoAlgorithms.KeyLengths(algorithm).Contains(keyLength))
            throw new InvalidOperationException(
                $"Недопустимый размер/параметр {keyLength} для {EdoAlgorithms.KeyLabel(algorithm)}.");
        if (defaultSignatureAlgorithm is { } signatureAlgorithm &&
            !_crypto.GetCompatibleSignatureAlgorithms(algorithm).Contains(signatureAlgorithm))
            throw new InvalidOperationException(
                $"Алгоритм подписи {EdoAlgorithms.SignatureLabel(signatureAlgorithm)} несовместим с {EdoAlgorithms.KeyLabel(algorithm)}.");

        var pair = await _crypto.GenerateKeyPairAsync(algorithm, keyLength, token);
        var now = DateTimeOffset.Now;
        var key = new EdoKey
        {
            Id = NewId(),
            Owner = owner,
            Algorithm = EdoAlgorithms.ToLegacy(algorithm),
            KeygenAlgorithm = algorithm,
            KeyLength = keyLength,
            DefaultSignatureAlgorithm = defaultSignatureAlgorithm,
            Status = EdoKeyStatus.Active,
            PublicKeySpki = pair.PublicKeyBase64,
            Fingerprint = pair.Fingerprint,
            CreatedAt = now,
            ValidFrom = now,
            ValidUntil = now.AddDays(days),
        };

        _state.Keys.Add(key);
        _privateKeys[key.Id] = Convert.FromBase64String(pair.PrivateKeyBase64);
        var signatureLabel = defaultSignatureAlgorithm is { } sig
            ? $", подпись {EdoAlgorithms.SignatureLabel(sig)}"
            : string.Empty;
        await LogEventAsync(
            "Ключи",
            $"Создана пара {EdoAlgorithms.KeyLabel(algorithm)} ({keyLength}){signatureLabel} для «{owner}», срок {days} дн.",
            token);

        return key;
    }

    public IReadOnlyList<EnumSignedAlgorithm> GetCompatibleSignatureAlgorithms(EdoKey key) =>
        _crypto.GetCompatibleSignatureAlgorithms(EdoAlgorithms.KeyAlgorithm(key));

    public EnumSignedAlgorithm? GetDefaultSignatureAlgorithm(EdoKey key)
    {
        var compatible = GetCompatibleSignatureAlgorithms(key);
        if (key.DefaultSignatureAlgorithm is { } configured && compatible.Contains(configured))
            return configured;

        var fromCrypto = _crypto.GetDefaultSignatureAlgorithm(EdoAlgorithms.KeyAlgorithm(key));
        if (fromCrypto is { } fallback && compatible.Contains(fallback))
            return fallback;

        return compatible.Count > 0 ? compatible[0] : null;
    }

    public async Task RevokeKeyAsync(
        string id, EnumRevokedReason reason = EnumRevokedReason.Unspecified,
        CancellationToken token = default)
    {
        var key = FindKey(id) ?? throw new InvalidOperationException("Ключ не найден.");
        if (key.Status != EdoKeyStatus.Active) return;

        key.Status = EdoKeyStatus.Revoked;
        key.RevokedAt = DateTimeOffset.Now;
        key.RevokedReason = reason;
        _privateKeys.Remove(key.Id);

        var shortFingerprint = key.Fingerprint.Length > 23 ? key.Fingerprint[..23] : key.Fingerprint;
        await LogEventAsync("Ключи", $"Ключ {shortFingerprint}… ({key.Owner}) отозван ({EdoLabels.RevokedReason(reason)})", token);
    }

    public async Task DeleteKeyAsync(string id, CancellationToken token = default)
    {
        var key = FindKey(id) ?? throw new InvalidOperationException("Ключ не найден.");
        _state.Keys.Remove(key);
        _privateKeys.Remove(key.Id);
        await LogEventAsync("Ключи", $"Ключ {ShortFingerprint(key.Fingerprint)} ({key.Owner}) удалён", token);
    }

    public async Task AttestKeyAsync(
        string id, string attestationBase64, int? remoteOwnerId = null,
        CancellationToken token = default)
    {
        var key = FindKey(id) ?? throw new InvalidOperationException("Ключ не найден.");
        if (key.Status != EdoKeyStatus.Active)
            throw new InvalidOperationException("Нельзя аттестовать отозванный ключ.");
        if (!string.IsNullOrEmpty(key.AttestationBase64))
            throw new InvalidOperationException("Аттестация для ключа уже существует.");

        byte[] attestation;
        try
        {
            attestation = Convert.FromBase64String(attestationBase64.Trim());
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("Аттестация должна быть корректной Base64-строкой.");
        }

        if (attestation.Length == 0)
            throw new InvalidOperationException("Аттестация не может быть пустой.");

        key.AttestationBase64 = Convert.ToBase64String(attestation);
        key.AttestedAt = DateTimeOffset.Now;
        key.RemoteOwnerId = remoteOwnerId;

        await LogEventAsync(
            "Ключи",
            $"Ключ {ShortFingerprint(key.Fingerprint)} ({key.Owner}) аттестован",
            token);
    }

    public string GetPublicKeyPem(string id)
    {
        var key = FindKey(id) ?? throw new InvalidOperationException("Ключ не найден.");
        var chunks = Enumerable.Range(0, (key.PublicKeySpki.Length + 63) / 64)
            .Select(i => key.PublicKeySpki.Substring(i * 64, Math.Min(64, key.PublicKeySpki.Length - i * 64)));

        return "-----BEGIN PUBLIC KEY-----\n" + string.Join('\n', chunks) + "\n-----END PUBLIC KEY-----";
    }

    public Task<EdoSignature> SignDocumentAsync(
        string documentId, string keyId, CancellationToken token = default)
    {
        var key = FindKey(keyId) ?? throw new InvalidOperationException("Ключ не найден.");
        var signatureAlgorithm = GetDefaultSignatureAlgorithm(key)
            ?? throw new InvalidOperationException("Для ключа нет доступных алгоритмов подписи.");
        return SignDocumentAsync(documentId, keyId, signatureAlgorithm, token);
    }

    public async Task<EdoSignature> SignDocumentAsync(
        string documentId, string keyId, EnumSignedAlgorithm signatureAlgorithm,
        CancellationToken token = default)
    {
        var document = FindDocument(documentId) ?? throw new InvalidOperationException("Документ не найден.");
        var key = FindKey(keyId) ?? throw new InvalidOperationException("Ключ не найден.");

        if (!KeyIsActive(key))
            throw new InvalidOperationException("Ключ отозван или истёк.");
        if (!_privateKeys.TryGetValue(keyId, out var privateKey))
            throw new InvalidOperationException("Закрытый ключ отсутствует в текущей сессии.");
        if (!GetCompatibleSignatureAlgorithms(key).Contains(signatureAlgorithm))
            throw new InvalidOperationException(
                $"Алгоритм {EdoAlgorithms.SignatureLabel(signatureAlgorithm)} несовместим с ключом {EdoAlgorithms.KeyLabel(EdoAlgorithms.KeyAlgorithm(key))}.");

        var canonical = CanonicalDocument(document.Number, document.Title, document.Author, document.Content);
        var hash = await _crypto.ComputeSha256Async(canonical, token);
        var signature = await _crypto.SignAsync(signatureAlgorithm, canonical, privateKey, token);

        var record = new EdoSignature
        {
            Id = NewId(),
            DocumentId = document.Id,
            KeyId = key.Id,
            Algorithm = EdoAlgorithms.ToLegacy(EdoAlgorithms.KeyAlgorithm(key)),
            SignedAlgorithm = signatureAlgorithm,
            Hash = hash,
            SignatureValue = Convert.ToBase64String(signature),
            SignedAt = DateTimeOffset.Now,
        };

        _state.Signatures.Add(record);
        await LogEventAsync(
            "Подпись",
            $"Документ {document.Number} подписан: {key.Owner}, {EdoAlgorithms.SignatureLabel(signatureAlgorithm)}",
            token);

        return record;
    }

    public async Task RevokeSignatureAsync(
        string signatureId, EnumRevokedReason reason, CancellationToken token = default)
    {
        var signature = FindSignature(signatureId)
            ?? throw new InvalidOperationException("Подпись не найдена.");
        if (signature.RevokedAt is not null) return;

        signature.RevokedAt = DateTimeOffset.Now;
        signature.RevokedReason = reason;
        await LogEventAsync("Подпись", $"Подпись {signature.Id} отозвана ({EdoLabels.RevokedReason(reason)})", token);
    }

    public async Task DeleteSignatureAsync(string signatureId, CancellationToken token = default)
    {
        var signature = FindSignature(signatureId)
            ?? throw new InvalidOperationException("Подпись не найдена.");

        _state.Signatures.Remove(signature);
        await LogEventAsync("Подпись", $"Подпись {signature.Id} удалена из реестра", token);
    }

    public async Task<VerificationResult> VerifySignatureAsync(
        string signatureId, CancellationToken token = default)
    {
        var signature = FindSignature(signatureId)
            ?? throw new InvalidOperationException("Подпись не найдена.");
        var document = FindDocument(signature.DocumentId);
        var key = FindKey(signature.KeyId);

        if (document is null || key is null)
        {
            var missing = BuildResult(
                false, false, false, false, false, signature.RevokedAt is not null,
                "—", "—", signature.SignedAt, signature.Hash);
            await LogEventAsync("Проверка", "Подпись не найдена в реестре", token);
            return missing;
        }

        var canonical = CanonicalDocument(document.Number, document.Title, document.Author, document.Content);
        var actualHash = await _crypto.ComputeSha256Async(canonical, token);
        var hashMatch = string.Equals(actualHash, signature.Hash, StringComparison.OrdinalIgnoreCase);

        var signatureOk = false;
        var signedAlgorithm = EdoAlgorithms.SignedAlgorithm(signature);
        try
        {
            signatureOk = await _crypto.VerifyAsync(
                signedAlgorithm,
                canonical,
                Convert.FromBase64String(signature.SignatureValue),
                Convert.FromBase64String(key.PublicKeySpki),
                token);
        }
        catch
        {
            signatureOk = false;
        }

        var revoked = key.Status == EdoKeyStatus.Revoked;
        var expired = key.ValidUntil < DateTimeOffset.Now;
        var keyOk = !revoked && !expired;

        var result = BuildResult(
            signatureOk, hashMatch, keyOk, revoked, expired, signature.RevokedAt is not null,
            key.Owner, EdoAlgorithms.SignatureLabel(signedAlgorithm), signature.SignedAt, signature.Hash);

        await LogEventAsync(
            "Проверка",
            $"Документ {document.Number}: {(result.IsValid ? "подпись действительна" : "проверка не пройдена")}",
            token);

        return result;
    }

    public async Task<VerificationResult> VerifyPackageAsync(
        string json, string? sourceName = null, CancellationToken token = default)
    {
        SignaturePackage package;
        try
        {
            package = JsonSerializer.Deserialize<SignaturePackage>(json, PackageJson)
                ?? throw new FormatException("Пустой пакет подписи.");
        }
        catch (JsonException ex)
        {
            throw new FormatException("Некорректный JSON пакета.", ex);
        }

        if (!string.Equals(package.Format, "edo-signature/2", StringComparison.Ordinal))
            throw new FormatException("Неизвестный формат пакета.");
        if (package.Document is null || package.Signature is null || package.PublicKey is null)
            throw new FormatException("В пакете отсутствуют обязательные разделы.");
        if (string.IsNullOrWhiteSpace(package.Document.Content) ||
            string.IsNullOrWhiteSpace(package.Signature.Value) ||
            string.IsNullOrWhiteSpace(package.PublicKey.Spki))
            throw new FormatException("В пакете отсутствуют обязательные данные.");

        var canonical = CanonicalDocument(
            package.Document.Number, package.Document.Title,
            package.Document.Author, package.Document.Content);
        var actualHash = await _crypto.ComputeSha256Async(canonical, token);
        var hashMatch = string.Equals(actualHash, package.Document.Sha256, StringComparison.OrdinalIgnoreCase);

        var signatureAlgorithmValue = !string.IsNullOrWhiteSpace(package.Signature.SignatureAlgorithm)
            ? package.Signature.SignatureAlgorithm
            : package.Signature.Algorithm;
        var signedAlgorithm = EdoAlgorithms.TryParseSignatureAlgorithm(signatureAlgorithmValue);
        if (signedAlgorithm is null &&
            EdoAlgorithms.TryParseKeyAlgorithm(package.Signature.Algorithm) is { } packageKeygen)
            signedAlgorithm = EdoAlgorithms.DefaultSignatureAlgorithm(packageKeygen);

        var signatureOk = false;
        try
        {
            var signatureBytes = Convert.FromBase64String(package.Signature.Value);
            var publicKeyBytes = Convert.FromBase64String(package.PublicKey.Spki);

            if (string.Equals(package.Signature.SigFormat, "P1363", StringComparison.OrdinalIgnoreCase))
            {
                var keygen = (signedAlgorithm is { } parsedSigned
                                 ? EdoAlgorithms.KeygenFromSignature(parsedSigned)
                                 : null)
                             ?? EdoAlgorithms.TryParseKeyAlgorithm(package.Signature.Algorithm)
                             ?? throw new FormatException("Не удалось определить алгоритм ключа для пакета.");

                var legacy = keygen is EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS
                    ? EdoKeyAlgorithm.Rsa
                    : keygen is EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA or EnumKpgAlgorithm.ECDH
                        or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV
                        ? EdoKeyAlgorithm.Ecdsa
                        : throw new FormatException("Предварительно вычисленная подпись поддерживается только для RSA и ECDSA.");

                var hashBytes = Convert.FromHexString(package.Document.Sha256);
                signatureOk = await _crypto.VerifyPrehashedAsync(
                    legacy, hashBytes, signatureBytes, publicKeyBytes, true, token);
            }
            else
            {
                if (signedAlgorithm is not { } algorithm)
                    throw new FormatException("Неизвестный алгоритм подписи в пакете.");
                signatureOk = await _crypto.VerifyAsync(
                    algorithm,
                    canonical,
                    signatureBytes,
                    publicKeyBytes,
                    token);
            }
        }
        catch
        {
            signatureOk = false;
        }

        var revoked = string.Equals(package.PublicKey.Status, "revoked", StringComparison.OrdinalIgnoreCase);
        var expired = package.PublicKey.ValidUntil != default && package.PublicKey.ValidUntil < DateTimeOffset.Now;
        var keyOk = !revoked && !expired;

        var result = BuildResult(
            signatureOk, hashMatch, keyOk, revoked, expired, false,
            package.PublicKey.Owner,
            signedAlgorithm is { } resultAlgorithm
                ? EdoAlgorithms.SignatureLabel(resultAlgorithm)
                : package.Signature.Algorithm,
            package.Signature.SignedAt,
            package.Document.Sha256);

        var name = string.IsNullOrWhiteSpace(sourceName) ? "пакет" : $"«{sourceName}»";
        await LogEventAsync(
            "Проверка",
            $"Пакет {name}: {(result.IsValid ? "подпись действительна" : "проверка не пройдена")}",
            token);

        return result;
    }

    public SignaturePackage BuildSignaturePackage(string signatureId)
    {
        var signature = FindSignature(signatureId)
            ?? throw new InvalidOperationException("Подпись не найдена.");
        var document = FindDocument(signature.DocumentId)
            ?? throw new InvalidOperationException("Документ не найден.");
        var key = FindKey(signature.KeyId)
            ?? throw new InvalidOperationException("Ключ не найден.");

        return new SignaturePackage
        {
            Format = "edo-signature/2",
            Document = new PackageDocument
            {
                Number = document.Number,
                Title = document.Title,
                Author = document.Author,
                Content = document.Content,
                Sha256 = signature.Hash,
            },
            Signature = new PackageSignature
            {
                Algorithm = EdoAlgorithms.KeyLabel(EdoAlgorithms.KeyAlgorithm(key)),
                SignatureAlgorithm = EdoAlgorithms.SignedAlgorithm(signature).ToString(),
                SigFormat = "DER",
                Value = signature.SignatureValue,
                SignedAt = signature.SignedAt,
            },
            PublicKey = new PackagePublicKey
            {
                Owner = key.Owner,
                Spki = key.PublicKeySpki,
                Fingerprint = key.Fingerprint,
                ValidFrom = key.ValidFrom,
                ValidUntil = key.ValidUntil,
                Status = key.Status == EdoKeyStatus.Revoked ? "revoked" : "active",
            },
        };
    }

    public string BuildSignaturePackageJson(string signatureId) =>
        JsonSerializer.Serialize(BuildSignaturePackage(signatureId), PackageJson);

    public string ExportRegistryJson() => JsonSerializer.Serialize(_state, PackageJson);

    public async Task<EdoTrackingFile> AddTrackingFileAsync(
        string path, string extension, CancellationToken token = default)
    {
        var normalizedPath = path.Trim();
        if (string.IsNullOrWhiteSpace(normalizedPath))
            throw new InvalidOperationException("Укажите путь к файлу.");

        var normalizedExtension = extension.Trim();
        if (!string.IsNullOrEmpty(normalizedExtension) && !normalizedExtension.StartsWith('.'))
            normalizedExtension = "." + normalizedExtension;

        if (_state.TrackingFiles.Any(f =>
                f.DeletedAt is null &&
                string.Equals(f.Path, normalizedPath, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Файл с таким путём уже отслеживается.");

        var file = new EdoTrackingFile
        {
            Id = NewId(),
            Path = normalizedPath,
            FixedExtension = normalizedExtension,
            StatusTracking = GetFileStatus(normalizedPath),
            CreatedAt = DateTimeOffset.Now,
        };

        _state.TrackingFiles.Add(file);
        await LogEventAsync("Файлы", $"Добавлен отслеживаемый файл «{file.Path}»", token);
        return file;
    }

    public async Task<EdoTrackingFile> CheckTrackingFileAsync(
        string id, CancellationToken token = default)
    {
        var file = FindTrackingFile(id) ?? throw new InvalidOperationException("Файл не найден.");
        file.StatusTracking = GetFileStatus(file.Path);
        await LogEventAsync(
            "Файлы",
            $"Проверка «{file.Path}»: {StatusTrackingLabel(file.StatusTracking)}",
            token);
        return file;
    }

    public async Task<EdoTrackingFile> SyncTrackingFileAsync(
        string id, CancellationToken token = default)
    {
        var file = FindTrackingFile(id) ?? throw new InvalidOperationException("Файл не найден.");
        file.StatusTracking = GetFileStatus(file.Path);
        file.LastSync = DateTimeOffset.Now;
        await LogEventAsync(
            "Синхронизация",
            $"«{file.Path}»: {StatusTrackingLabel(file.StatusTracking)}",
            token);
        return file;
    }

    public async Task PauseTrackingFileAsync(string id, CancellationToken token = default)
    {
        var file = FindTrackingFile(id) ?? throw new InvalidOperationException("Файл не найден.");
        file.StatusTracking = EnumStatusTracking.Paused;
        await LogEventAsync("Файлы", $"Отслеживание «{file.Path}» приостановлено", token);
    }

    public async Task DeleteTrackingFileAsync(string id, CancellationToken token = default)
    {
        var file = FindTrackingFile(id) ?? throw new InvalidOperationException("Файл не найден.");
        _state.TrackingFiles.Remove(file);
        await LogEventAsync("Файлы", $"Запись «{file.Path}» удалена из реестра", token);
    }

    public async Task SoftDeleteTrackingFileAsync(string id, CancellationToken token = default)
    {
        var file = FindTrackingFile(id) ?? throw new InvalidOperationException("Файл не найден.");
        if (file.DeletedAt is not null) return;

        file.DeletedAt = DateTimeOffset.Now;
        await LogEventAsync("Файлы", $"Отслеживание «{file.Path}» удалено", token);
    }

    public async Task SaveSettingsAsync(EdoSettings settings, CancellationToken token = default)
    {
        if (!settings.AuthApiUrl.IsAbsoluteUri || !settings.BridgeApiUrl.IsAbsoluteUri)
            throw new InvalidOperationException("Адреса API должны быть абсолютными URL.");
        if (string.IsNullOrWhiteSpace(settings.RootPath))
            throw new InvalidOperationException("Укажите корневой каталог.");
        if (settings.SyncInterval <= TimeSpan.Zero)
            throw new InvalidOperationException("Интервал синхронизации должен быть больше нуля.");

        _state.Settings = settings;
        await LogEventAsync(
            "Настройки",
            $"Сохранены настройки: Auth={settings.AuthApiUrl}, Bridge={settings.BridgeApiUrl}, интервал={settings.SyncInterval}",
            token);
    }

    public string GetFullTrackingPath(EdoTrackingFile file)
    {
        var root = _state.Settings.RootPath;
        return Path.IsPathRooted(file.Path) ? file.Path : Path.GetFullPath(Path.Combine(root, file.Path));
    }

    private EnumStatusTracking GetFileStatus(string path)
    {
        try
        {
            var fullPath = Path.IsPathRooted(path)
                ? path
                : Path.GetFullPath(Path.Combine(_state.Settings.RootPath, path));
            return File.Exists(fullPath) ? EnumStatusTracking.Active : EnumStatusTracking.Missing;
        }
        catch
        {
            return EnumStatusTracking.Error;
        }
    }

    public static string StatusTrackingLabel(EnumStatusTracking status) => status switch
    {
        EnumStatusTracking.Active => "активен",
        EnumStatusTracking.HashesDontMatch => "хеши не совпадают",
        EnumStatusTracking.Missing => "файл отсутствует",
        EnumStatusTracking.Error => "ошибка",
        EnumStatusTracking.Paused => "приостановлен",
        _ => status.ToString(),
    };

    public async Task ClearAllAsync(CancellationToken token = default)
    {
        _state.Documents.Clear();
        _state.Keys.Clear();
        _state.Signatures.Clear();
        _state.Hashes.Clear();
        _state.TrackingFiles.Clear();
        _state.Log.Clear();
        _state.LogMac = null;
        _privateKeys.Clear();
        await _store.SaveAsync(_state, token);
    }

    public async Task SetCurrentUserAsync(string userId, CancellationToken token = default)
    {
        if (_state.SessionUserId == userId) return;

        _state.SessionUserId = EdoUsers.All.Any(u => u.Id == userId) ? userId : EdoUsers.All[0].Id;
        await LogEventAsync(
            "Сессия",
            $"Вход: {CurrentUser.Name} ({CurrentUser.Title})",
            token);
    }

    public async Task SetThemeAsync(string theme, CancellationToken token = default)
    {
        var value = theme is "light" or "dark" ? theme : "auto";
        if (_state.Theme == value) return;
        _state.Theme = value;
        await _store.SaveAsync(_state, token);
    }

    public async Task LogEventAsync(string type, string message, CancellationToken token = default)
    {
        var previous = _state.Log.Count > 0
            ? _state.Log[0].Chain ?? new string('0', 64)
            : new string('0', 64);

        var entry = new EdoLogEntry
        {
            Time = DateTimeOffset.Now,
            Type = type,
            Message = message,
            User = CurrentUser.Name,
        };

        var chainInput = previous + "|" + JsonSerializer.Serialize(entry, CompactJson);
        entry.Chain = HmacHex(chainInput, _sessionSecret);

        _state.Log.Insert(0, entry);
        if (_state.Log.Count > 500)
            _state.Log.RemoveRange(500, _state.Log.Count - 500);

        _state.LogMac = ComputeLogMac();
        await _store.SaveAsync(_state, token);
    }

    public bool VerifyLogMac()
    {
        if (_state.Log.Count == 0) return true;
        return string.Equals(ComputeLogMac(), _state.LogMac, StringComparison.Ordinal);
    }

    private string ComputeLogMac()
    {
        var body = JsonSerializer.Serialize(_state.Log, CompactJson);
        return HmacHex(body, _sessionSecret);
    }

    public static string CanonicalDocument(string number, string title, string author, string content)
    {
        var fields = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["author"] = author ?? string.Empty,
            ["content"] = content ?? string.Empty,
            ["number"] = number ?? string.Empty,
            ["title"] = title ?? string.Empty,
        };

        return JsonSerializer.Serialize(fields, CompactJson);
    }

    private static VerificationResult BuildResult(
        bool signatureOk, bool hashMatch, bool keyOk, bool revoked, bool expired,
        bool signatureRevoked, string owner, string algorithmLabel,
        DateTimeOffset time, string hash)
    {
        var verdict = signatureOk && hashMatch && keyOk && !signatureRevoked
            ? ("ok", "Подпись действительна")
            : revoked
                ? ("bad", "Ключ отозван")
                : signatureRevoked
                    ? ("bad", "Подпись отозвана")
                    : expired
                        ? ("bad", "Срок действия ключа истёк")
                        : !hashMatch
                            ? ("bad", "Документ изменён после подписания")
                            : !signatureOk
                                ? ("bad", "Подпись недействительна")
                                : ("bad", "Проверка не пройдена");

        return new VerificationResult
        {
            SignatureOk = signatureOk,
            HashMatch = hashMatch,
            KeyOk = keyOk,
            Revoked = revoked,
            Expired = expired,
            SignatureRevoked = signatureRevoked,
            VerdictKind = verdict.Item1,
            VerdictText = verdict.Item2,
            Owner = owner,
            AlgorithmLabel = algorithmLabel,
            Time = time,
            Hash = hash,
        };
    }

    private static string NewId()
    {
        var value = Guid.NewGuid().ToString("N");
        return value[..12];
    }

    private static string ShortHash(string hash) =>
        hash.Length > 12 ? hash[..12] + "…" : hash;

    private static string ShortFingerprint(string fingerprint) =>
        fingerprint.Length > 23 ? fingerprint[..23] + "…" : fingerprint;

    private static string HmacHex(string text, byte[] key) =>
        Convert.ToHexString(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
}
