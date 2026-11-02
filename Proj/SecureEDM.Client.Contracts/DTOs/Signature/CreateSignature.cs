using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.Signature;

public class CreateSignature(IClock clock)
{
    protected readonly IClock _clock = clock;
    public int DocumentId { get; set; }
    public int PublicKeyId { get; set; }
    public ImmutableArray<byte> SignatureValue { get; set; }
    public DateTimeOffset SignedAt { get; set; }
    public int? RemoteAuthorId { get; set; }
    public EnumSignedAlgorithm SignedAlgorithm { get; set; }
}