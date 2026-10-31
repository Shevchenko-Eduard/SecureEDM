using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class AsymmetricKeyOrm : AsymmetricKey
{
    protected AsymmetricKeyOrm(IClock clock) : base(clock) { }
    private AsymmetricKeyOrm() : base(GlobalClock.Clock) { }
    public PublicKeyOrm? PublicKey { get; private set; }
    public PrivateKeyOrm? PrivateKey { get; private set; }
}