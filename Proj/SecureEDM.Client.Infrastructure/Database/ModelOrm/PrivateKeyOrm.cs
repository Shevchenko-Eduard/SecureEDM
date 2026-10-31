using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class PrivateKeyOrm : PrivateKey
{
    protected PrivateKeyOrm(IClock clock) : base(clock) { }
    private PrivateKeyOrm() : base(GlobalClock.Clock) { }
    public AsymmetricKeyOrm? AsymmetricKey { get; private set; }
}