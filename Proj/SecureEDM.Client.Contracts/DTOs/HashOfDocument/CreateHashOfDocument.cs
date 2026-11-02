using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.HashOfDocument;

public class CreateHashOfDocument(IClock clock)
{
    protected readonly IClock _clock = clock;
    public DateTimeOffset CreatedAt { get; set; }
    public int DocumentId { get; set; }
    public int? RemoteAuthorId { get; set; }
    public EnumDigestAlgorithm HashedAlgorithm { get; set; }
    public ImmutableArray<byte> Hash { get; set; }
    public bool IsPrimary { get; set; }
}