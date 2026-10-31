using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class TrackingDocumentOrm : TrackingDocument
{
    protected TrackingDocumentOrm(IClock clock) : base(clock) { }
    private TrackingDocumentOrm() : base(GlobalClock.Clock) { }
    public HashOfDocumentOrm? HashOfDocument { get; private set; }
    public SignatureOrm? Signature { get; set; }
}