using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Cryptography.ValueObject;

namespace SecureEDM.Common.Infrastructure.Cryptography.Interfaces;

public interface ISignedAlgorithm
{
    ValueTask<byte[]> Signing(EnumSignedAlgorithm algorithm, Stream signable, byte[] privateKey, bool leaveInnerOpen = false, CancellationToken token = default);
    ValueTask<bool> Verify(Signature signature, Stream signable, bool leaveInnerOpen = false, CancellationToken token = default);
}