using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Cryptography.ValueObject;

namespace SecureEDM.Common.Infrastructure.Cryptography.Interfaces;

public interface IKpgAlgorithm
{
    ValueTask<KeyPair> Generate(EnumKpgAlgorithm algorithm, int lengthKey, CancellationToken token = default);
}