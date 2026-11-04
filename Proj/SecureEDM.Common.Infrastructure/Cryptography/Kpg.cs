using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Cryptography.Interfaces;
using SecureEDM.Common.Infrastructure.Cryptography.Utilities;
using SecureEDM.Common.Infrastructure.Cryptography.ValueObject;

namespace SecureEDM.Common.Infrastructure.Cryptography;

public class Kpg : IKpgAlgorithm
{
    public async ValueTask<KeyPair> Generate(
        EnumKpgAlgorithm algorithm, int lengthKey,
        CancellationToken token = default)
    {
        var kpg = KpgAlgorithmUtilities.GetKpg(algorithm);
        kpg.Init(kpg.GetParameters(algorithm, lengthKey));
        var keys = kpg.GenerateKeyPair();
        byte[] privateKeyBytes = PrivateKeyInfoFactory
                                                .CreatePrivateKeyInfo(keys.Private)
                                                .GetEncoded();
        byte[] publicKeyBytes = SubjectPublicKeyInfoFactory
                                                .CreateSubjectPublicKeyInfo(keys.Public)
                                                .GetEncoded();
        
        return new(publicKeyBytes, privateKeyBytes);
    }
}