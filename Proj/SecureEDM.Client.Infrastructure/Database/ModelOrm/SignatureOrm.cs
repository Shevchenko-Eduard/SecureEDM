using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class SignatureOrm : Signature
{
    protected SignatureOrm(IClock clock) : base(clock) { }
    private SignatureOrm() : base(GlobalClock.Clock) { }
    public TrackingDocumentOrm? TrackingDocument { get; private set; }
    public PublicKeyOrm? PublicKey { get; private set; }
}