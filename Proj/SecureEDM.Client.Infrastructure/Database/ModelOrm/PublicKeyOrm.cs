using System.Collections;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class PublicKeyOrm : PublicKey
{
    protected PublicKeyOrm(IClock clock) : base(clock) { }
    private PublicKeyOrm() : base(GlobalClock.Clock) { }
    public ICollection<SignatureOrm>? Signatures { get; private set; }
    public AsymmetricKeyOrm? AsymmetricKey { get; private set; }
}