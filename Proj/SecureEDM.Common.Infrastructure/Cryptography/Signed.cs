using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;
using SecureEDM.Common.Infrastructure.Cryptography.Utilities;

namespace SecureEDM.Common.Infrastructure.Cryptography;

public class Signed : ISignedAlgorithm
{
    public async ValueTask<byte[]> Signing(
        EnumSignedAlgorithm algorithm, Stream signable,
        byte[] privateKey, bool leaveInnerOpen = false,
        CancellationToken token = default)
    {
        var signed = SignetAlgorithmUtilities.GetSigner(algorithm);
        AsymmetricKeyParameter keyParam = PrivateKeyFactory.CreateKey(privateKey);
        signed.Init(true, keyParam);

        await ReadingStream.ReadInBuffer(
            signable, (buffer, _, bytesRead) => signed.BlockUpdate(buffer, 0, bytesRead),
            leaveInnerOpen, token);

        byte[] signature = signed.GenerateSignature();

        signed.Reset();

        return signature;
    }

    public async ValueTask<bool> Verify(
        CryptoSignature signature, Stream signable,
        bool leaveInnerOpen = false,
        CancellationToken token = default)
    {
        var signed = SignetAlgorithmUtilities.GetSigner(signature.Algorithm);
        signed.Init(false, PublicKeyFactory.CreateKey([.. signature.PublicKey]));

        await ReadingStream.ReadInBuffer(
            signable, (buffer, _, bytesRead) => signed.BlockUpdate(buffer, 0, bytesRead),
            leaveInnerOpen, token);

        return signed.VerifySignature([.. signature.Value]);
    }
}