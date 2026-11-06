using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Common.Contracts.Interfaces;

public interface IDigestAlgorithm
{
    ValueTask<byte[]> Hashing(EnumDigestAlgorithm algorithm, Stream stream, bool leaveInnerOpen = false, CancellationToken token = default);
}