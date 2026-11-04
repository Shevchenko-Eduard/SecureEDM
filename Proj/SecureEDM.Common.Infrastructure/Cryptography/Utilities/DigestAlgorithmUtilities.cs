using Org.BouncyCastle.Crypto;
using SecureEDM.Common.Domain.Enums;
using Org.BouncyCastle.Security;
using SecureEDM.Common.Infrastructure.Cryptography.Enums;

namespace SecureEDM.Common.Infrastructure.Cryptography.Utilities;

public static class DigestAlgorithmUtilities
{
    public static IDigest GetDigest(EnumDigestAlgorithm algorithm)
    {
        return DigestUtilities.GetDigest(MapperEnumDigestAlgorithm.Mapper.TryFromID(algorithm));
    }
}