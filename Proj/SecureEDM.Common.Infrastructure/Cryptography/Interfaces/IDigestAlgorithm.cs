using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Common.Infrastructure.Cryptography.Interfaces;

public interface IDigestAlgorithm
{
    ValueTask<byte[]> Hashing(EnumDigestAlgorithm algorithm, Stream stream, bool leaveInnerOpen = false, CancellationToken token = default);
}