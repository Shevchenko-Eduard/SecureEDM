using Org.BouncyCastle.Crypto;
using SecureEDM.Common.Domain.Enums;
using Org.BouncyCastle.Security;
using SecureEDM.Common.Infrastructure.Cryptography.Enums;

namespace SecureEDM.Common.Infrastructure.Cryptography.Utilities;

public static class SignetAlgorithmUtilities
{
    public static ISigner GetSigner(EnumSignedAlgorithm algorithm)
    {
        return SignerUtilities.GetSigner(MapperEnumSignedAlgorithm.Mapper.TryFromID(algorithm));
    }
}