using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Models.ValueObject;
using SecureEDM.Common.Infrastructure.Cryptography.Utilities;

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