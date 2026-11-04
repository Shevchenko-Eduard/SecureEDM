using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Cryptography.Interfaces;
using SecureEDM.Common.Infrastructure.Cryptography.Utilities;

namespace SecureEDM.Common.Infrastructure.Cryptography;

public class Digest : IDigestAlgorithm
{
    public async ValueTask<byte[]> Hashing(
        EnumDigestAlgorithm algorithm,
        Stream stream, bool leaveInnerOpen = false,
        CancellationToken token = default)
    {
        var digest = DigestAlgorithmUtilities.GetDigest(algorithm);
        byte[] hash = new byte[digest.GetDigestSize()];
        await ReadingStream.ReadInBuffer(
            stream, (buffer, _, bytesRead) => digest.BlockUpdate(buffer, 0, bytesRead),
            leaveInnerOpen, token);
        digest.DoFinal(hash);
        return hash;
    }
}