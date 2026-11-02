using System.Collections.Immutable;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.PublicKey;

public class CreatePublicKey(IClock clock)
{
    protected readonly IClock _clock = clock;
    public int AsymmetricKeyId { get; set; }
    public int? RemoteOwnerId { get; set; }
    public ImmutableArray<byte> KeyValue { get; set; }
}