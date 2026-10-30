using Org.BouncyCastle.Security;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Common.Infrastructure;

public class Clock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    // public void hui()
    // {
    //     SignerUtilities.GetSigner("");
    //     DigestUtilities.GetDigest("");
    //     // Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator;
    // }
}