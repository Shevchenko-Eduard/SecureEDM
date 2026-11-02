using System.Collections.Immutable;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.PrivateKey;

public class CreatePrivateKey(IClock clock)
{
    protected readonly IClock _clock = clock;
    public int AsymmetricKeyId { get; set; }
    public ImmutableArray<byte> KeyValue { get; set; }
}