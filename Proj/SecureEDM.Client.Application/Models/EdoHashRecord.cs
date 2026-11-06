using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Application.Models;

/// <summary>Хеш документа, соответствующий HashOfDocument из доменной модели клиента.</summary>
public sealed class EdoHashRecord
{
    public string Id { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public EnumDigestAlgorithm Algorithm { get; set; } = EnumDigestAlgorithm.SHA_256;
    public string HashHex { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public EnumRevokedReason RevokedReason { get; set; }
    public int? RemoteAuthorId { get; set; }
}
