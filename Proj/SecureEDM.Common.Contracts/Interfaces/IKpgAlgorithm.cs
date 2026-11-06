using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Common.Contracts.Interfaces;

public interface IKpgAlgorithm
{
    ValueTask<KeyPair> Generate(EnumKpgAlgorithm algorithm, int lengthKey, CancellationToken token = default);
}