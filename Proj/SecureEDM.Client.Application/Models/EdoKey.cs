using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Application.Models;

public enum EdoKeyAlgorithm
{
    Rsa,
    Ecdsa,
}

public enum EdoKeyStatus
{
    Active,
    Revoked,
}

/// <summary>Запись реестра открытых ключей. Закрытый ключ хранится только в памяти сессии.</summary>
public sealed class EdoKey
{
    public string Id { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;

    /// <summary>Старое поле совместимости с первыми версиями реестра.</summary>
    public EdoKeyAlgorithm Algorithm { get; set; }

    public EnumKpgAlgorithm? KeygenAlgorithm { get; set; }
    public int KeyLength { get; set; }
    public EnumSignedAlgorithm? DefaultSignatureAlgorithm { get; set; }
    public EdoKeyStatus Status { get; set; } = EdoKeyStatus.Active;

    /// <summary>Открытый ключ в формате SubjectPublicKeyInfo (DER), Base64.</summary>
    public string PublicKeySpki { get; set; } = string.Empty;

    public string Fingerprint { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset ValidUntil { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public EnumRevokedReason RevokedReason { get; set; }
    public string? AttestationBase64 { get; set; }
    public DateTimeOffset? AttestedAt { get; set; }
    public int? RemoteOwnerId { get; set; }
}

public sealed class EdoSignature
{
    public string Id { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;

    /// <summary>Старое поле совместимости; для новых подписей используйте SignedAlgorithm.</summary>
    public EdoKeyAlgorithm Algorithm { get; set; }

    public EnumSignedAlgorithm? SignedAlgorithm { get; set; }

    /// <summary>SHA-256 канонизированного документа на момент подписания.</summary>
    public string Hash { get; set; } = string.Empty;

    /// <summary>Значение подписи (DER), Base64.</summary>
    public string SignatureValue { get; set; } = string.Empty;

    public DateTimeOffset SignedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public EnumRevokedReason RevokedReason { get; set; }
    public int? RemoteAuthorId { get; set; }
}

public sealed class EdoLogEntry
{
    public DateTimeOffset Time { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string? Chain { get; set; }
}

/// <summary>Состояние, которое сохраняется между запусками приложения.</summary>
public sealed class EdoPersistedState
{
    public List<EdoDocument> Documents { get; set; } = [];
    public List<EdoKey> Keys { get; set; } = [];
    public List<EdoSignature> Signatures { get; set; } = [];
    public List<EdoHashRecord> Hashes { get; set; } = [];
    public List<EdoTrackingFile> TrackingFiles { get; set; } = [];
    public EdoSettings Settings { get; set; } = new();
    public List<EdoLogEntry> Log { get; set; } = [];
    public string? LogMac { get; set; }
    public string SessionUserId { get; set; } = "u1";
    public string SessionSecretBase64 { get; set; } = string.Empty;
    public string Theme { get; set; } = "auto";
}
