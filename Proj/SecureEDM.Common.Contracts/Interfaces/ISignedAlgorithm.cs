using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Common.Contracts.Interfaces;

public interface ISignedAlgorithm
{
    ValueTask<byte[]> Signing(EnumSignedAlgorithm algorithm, Stream signable, byte[] privateKey, bool leaveInnerOpen = false, CancellationToken token = default);
    ValueTask<bool> Verify(CryptoSignature signature, Stream signable, bool leaveInnerOpen = false, CancellationToken token = default);
}