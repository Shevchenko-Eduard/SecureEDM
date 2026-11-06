using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SecureEDM.Client.Application.Interfaces;
using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Client.Application.Models;
using SecureEDM.Client.Application.Services;
using SecureEDM.Client.Infrastructure.Cryptography;

namespace SecureEDM.Client.Application;

public class EdoServiceTest
{
    private sealed class MemoryStore : IRegistryStore
    {
        public EdoPersistedState State { get; set; } = new();

        public ValueTask<EdoPersistedState?> LoadAsync(CancellationToken token = default) =>
            ValueTask.FromResult<EdoPersistedState?>(State);

        public ValueTask SaveAsync(EdoPersistedState state, CancellationToken token = default)
        {
            State = state;
            return ValueTask.CompletedTask;
        }
    }

    private static EdoService CreateService()
    {
        var service = new EdoService(new MemoryStore(), new ClientCrypto());
        service.InitializeAsync().GetAwaiter().GetResult();
        return service;
    }

    [Fact]
    public async Task DocumentCanBeSignedAndVerified()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Приказ", "Иванов И. И.", "Текст приказа");
        var key = await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);

        var signature = await service.SignDocumentAsync(document.Id, key.Id);
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.True(result.IsValid);
        Assert.True(result.SignatureOk);
        Assert.True(result.HashMatch);
        Assert.True(result.KeyOk);
    }

    [Fact]
    public async Task EcdsaDocumentCanBeSignedAndVerified()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Договор", "Петрова А. С.", "Текст договора");
        var key = await service.GenerateKeyAsync("Петрова А. С.", EdoKeyAlgorithm.Ecdsa, 30);

        var signature = await service.SignDocumentAsync(document.Id, key.Id);
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task PackageVerificationWorksAndDetectsTampering()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Акт", "Иванов И. И.", "Текст акта");
        var key = await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);
        var signature = await service.SignDocumentAsync(document.Id, key.Id);

        var package = service.BuildSignaturePackageJson(signature.Id);
        var valid = await service.VerifyPackageAsync(package, "act.json");
        Assert.True(valid.IsValid);

        var tampered = package.Replace("Текст акта", "Изменённый текст", StringComparison.Ordinal);
        var invalid = await service.VerifyPackageAsync(tampered, "act-tampered.json");
        Assert.False(invalid.IsValid);
    }


    [Fact]
    public async Task HtmlStylePrehashedPackagesAreSupported()
    {
        var service = CreateService();

        using var rsa = RSA.Create(2048);
        const string content = "Текст для HTML-пакета";
        var canonical = EdoService.CanonicalDocument("Д-777", "Пакет", "Иванов И. И.", content);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        var signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        var package = new SignaturePackage
        {
            Document = new PackageDocument
            {
                Number = "Д-777",
                Title = "Пакет",
                Author = "Иванов И. И.",
                Content = content,
                Sha256 = Convert.ToHexString(hash).ToLowerInvariant(),
            },
            Signature = new PackageSignature
            {
                Algorithm = "RSA",
                SigFormat = "P1363",
                Value = Convert.ToBase64String(signature),
                SignedAt = DateTimeOffset.Now,
            },
            PublicKey = new PackagePublicKey
            {
                Owner = "Иванов И. И.",
                Spki = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
                Fingerprint = "00:11",
                ValidFrom = DateTimeOffset.Now.AddDays(-1),
                ValidUntil = DateTimeOffset.Now.AddDays(365),
                Status = "active",
            },
        };

        var json = JsonSerializer.Serialize(package);
        var result = await service.VerifyPackageAsync(json, "html-package.json");

        Assert.True(result.IsValid);
        Assert.True(result.SignatureOk);
    }

    [Fact]
    public async Task DocumentChangeInvalidatesSignature()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Приказ", "Иванов И. И.", "Исходный текст");
        var key = await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);
        var signature = await service.SignDocumentAsync(document.Id, key.Id);

        await service.UpdateDocumentAsync(document.Id, document.Title, document.Author, "Новый текст");
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.False(result.IsValid);
        Assert.False(result.HashMatch);
    }

    [Fact]
    public async Task RevokedKeyInvalidatesSignature()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Приказ", "Иванов И. И.", "Текст");
        var key = await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);
        var signature = await service.SignDocumentAsync(document.Id, key.Id);

        await service.RevokeKeyAsync(key.Id);
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.False(result.IsValid);
        Assert.True(result.Revoked);
        Assert.False(result.KeyOk);
    }

    [Fact]
    public async Task LogMacDetectsTampering()
    {
        var service = CreateService();
        await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);

        Assert.True(service.VerifyLogMac());
        service.Log[0].Message = "Подменённое событие";
        Assert.False(service.VerifyLogMac());
    }

    [Fact]
    public async Task PermissionsFollowCurrentUserRole()
    {
        var service = CreateService();

        await service.SetCurrentUserAsync("u2");
        Assert.False(service.Can("editDoc"));
        Assert.False(service.Can("sign"));
        Assert.False(service.Can("export"));

        await service.SetCurrentUserAsync("u3");
        Assert.True(service.Can("editDoc"));
        Assert.True(service.Can("clear"));
        Assert.True(service.Can("export"));
    }



    [Fact]
    public async Task Ed25519KeyCanSignAndVerify()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Документ", "Иванов И. И.", "Текст");
        var key = await service.GenerateKeyAsync(
            "Иванов И. И.", EnumKpgAlgorithm.Ed25519, 255, EnumSignedAlgorithm.Ed25519, 365);

        var signature = await service.SignDocumentAsync(
            document.Id, key.Id, EnumSignedAlgorithm.Ed25519);
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.True(result.IsValid);
        Assert.Equal(EnumSignedAlgorithm.Ed25519, signature.SignedAlgorithm);
    }

    [Fact]
    public async Task DsaAndGostKeysCanSign()
    {
        var service = CreateService();

        var dsaDocument = await service.CreateDocumentAsync("DSA", "Иванов И. И.", "Текст");
        var dsaKey = await service.GenerateKeyAsync(
            "Иванов И. И.", EnumKpgAlgorithm.DSA, 1024, EnumSignedAlgorithm.SHA_256withDSA, 365);
        var dsaSignature = await service.SignDocumentAsync(
            dsaDocument.Id, dsaKey.Id, EnumSignedAlgorithm.SHA_256withDSA);
        Assert.True((await service.VerifySignatureAsync(dsaSignature.Id)).IsValid);

        var gostDocument = await service.CreateDocumentAsync("GOST", "Иванов И. И.", "Текст");
        var gostKey = await service.GenerateKeyAsync(
            "Иванов И. И.", EnumKpgAlgorithm.GOST3410, 256, EnumSignedAlgorithm.GOST3410, 365);
        var gostSignature = await service.SignDocumentAsync(
            gostDocument.Id, gostKey.Id, EnumSignedAlgorithm.GOST3410);
        Assert.True((await service.VerifySignatureAsync(gostSignature.Id)).IsValid);
    }

    [Fact]
    public async Task RsaSupportsAlternativeSignatureAlgorithms()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Документ", "Иванов И. И.", "Текст");
        var key = await service.GenerateKeyAsync(
            "Иванов И. И.", EnumKpgAlgorithm.RSA, 2048,
            EnumSignedAlgorithm.SHA_512withRSA, 365);

        var signature = await service.SignDocumentAsync(
            document.Id, key.Id, EnumSignedAlgorithm.SHA_512withRSA);
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.True(result.IsValid);
        Assert.Equal("SHA-512withRSA", EdoAlgorithms.SignatureLabel(signature.SignedAlgorithm!.Value));
    }

    [Fact]
    public async Task NonSigningKeyAlgorithmsAreAvailableWithoutSignatures()
    {
        var service = CreateService();
        var key = await service.GenerateKeyAsync(
            "Иванов И. И.", EnumKpgAlgorithm.ML_KEM, 512, null, 365);

        Assert.Equal(EnumKpgAlgorithm.ML_KEM, key.KeygenAlgorithm);
        Assert.Empty(service.GetCompatibleSignatureAlgorithms(key));
        Assert.Null(service.GetDefaultSignatureAlgorithm(key));
    }

    [Fact]
    public async Task HashCanBeCalculatedWithAllProjectAlgorithms()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Документ", "Иванов И. И.", "Текст");

        foreach (var algorithm in Enum.GetValues<EnumDigestAlgorithm>())
        {
            var hash = await service.AddHashAsync(document.Id, algorithm);
            Assert.False(string.IsNullOrWhiteSpace(hash.HashHex));
            Assert.Equal(algorithm, hash.Algorithm);
        }
    }

    [Fact]
    public async Task HashRegistrySupportsAlternativeAlgorithmsAndRevocation()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Документ", "Иванов И. И.", "Текст");

        Assert.Single(service.Hashes);
        Assert.True(service.Hashes[0].IsPrimary);

        var sha512 = await service.AddHashAsync(document.Id, EnumDigestAlgorithm.SHA_512);
        Assert.Equal(2, service.Hashes.Count);
        Assert.False(sha512.IsPrimary);

        await service.SetPrimaryHashAsync(sha512.Id);
        Assert.True(service.Hashes.Single(h => h.Id == sha512.Id).IsPrimary);
        Assert.False(service.Hashes.Single(h => h.Algorithm == EnumDigestAlgorithm.SHA_256).IsPrimary);

        await service.RevokeHashAsync(sha512.Id, EnumRevokedReason.Cancelled);
        Assert.NotNull(service.Hashes.Single(h => h.Id == sha512.Id).RevokedAt);

        await service.DeleteHashAsync(sha512.Id);
        Assert.Single(service.Hashes);
    }

    [Fact]
    public async Task DocumentUpdateCreatesHistoryAndRotatesHashes()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Документ", "Иванов И. И.", "Версия 1");
        var oldHash = service.Hashes.Single(h => h.IsPrimary);

        await service.UpdateDocumentAsync(document.Id, document.Title, document.Author, "Версия 2");

        Assert.Single(document.History);
        Assert.Equal(1, document.Version);
        Assert.NotNull(service.Hashes.Single(h => h.Id == oldHash.Id).RevokedAt);
        Assert.Single(service.Hashes, h => h.IsPrimary && h.RevokedAt is null);
    }

    [Fact]
    public async Task SignatureRevocationInvalidatesVerification()
    {
        var service = CreateService();
        var document = await service.CreateDocumentAsync("Документ", "Иванов И. И.", "Текст");
        var key = await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);
        var signature = await service.SignDocumentAsync(document.Id, key.Id);

        Assert.True((await service.VerifySignatureAsync(signature.Id)).IsValid);

        await service.RevokeSignatureAsync(signature.Id, EnumRevokedReason.Compromise);
        var result = await service.VerifySignatureAsync(signature.Id);

        Assert.False(result.IsValid);
        Assert.True(result.SignatureRevoked);
    }

    [Fact]
    public async Task KeyCanBeAttestedAndRevokedWithReason()
    {
        var service = CreateService();
        var key = await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);

        await service.AttestKeyAsync(
            key.Id, Convert.ToBase64String("attestation"u8.ToArray()), 7);

        Assert.NotNull(key.AttestationBase64);
        Assert.Equal(7, key.RemoteOwnerId);

        await service.RevokeKeyAsync(key.Id, EnumRevokedReason.Compromise);
        Assert.Equal(EdoKeyStatus.Revoked, key.Status);
        Assert.Equal(EnumRevokedReason.Compromise, key.RevokedReason);
    }

    [Fact]
    public async Task TrackingFilesSupportCheckSyncPauseAndSoftDelete()
    {
        var service = CreateService();
        var directory = Path.Combine(Path.GetTempPath(), "secureedm-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, "document.txt");
        await File.WriteAllTextAsync(filePath, "content");

        try
        {
            await service.SaveSettingsAsync(new EdoSettings { RootPath = directory });

            var file = await service.AddTrackingFileAsync("document.txt", ".txt");
            Assert.Equal(EnumStatusTracking.Active, file.StatusTracking);

            await service.SyncTrackingFileAsync(file.Id);
            Assert.NotNull(file.LastSync);

            await service.PauseTrackingFileAsync(file.Id);
            Assert.Equal(EnumStatusTracking.Paused, file.StatusTracking);

            File.Delete(filePath);
            await service.CheckTrackingFileAsync(file.Id);
            Assert.Equal(EnumStatusTracking.Missing, file.StatusTracking);

            await service.SoftDeleteTrackingFileAsync(file.Id);
            Assert.NotNull(file.DeletedAt);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task ClearAllKeepsSessionAndRemovesRegistry()
    {
        var service = CreateService();
        await service.CreateDocumentAsync("Приказ", "Иванов И. И.", "Текст");
        await service.GenerateKeyAsync("Иванов И. И.", EdoKeyAlgorithm.Rsa, 365);

        await service.ClearAllAsync();

        Assert.Empty(service.Documents);
        Assert.Empty(service.Keys);
        Assert.Empty(service.Signatures);
        Assert.Empty(service.Log);
        Assert.Equal("u1", service.SessionUserId);
    }
}
