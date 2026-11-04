using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Contracts.DTOs.AsymmetricKey;

public class CreateAsymmetricKey(IClock clock)
{
    protected readonly IClock _clock = clock;
    public DateTimeOffset CreatedAt { get; set; }
    public EnumKpgAlgorithm KeygenAlgorithm { get; set; }
}