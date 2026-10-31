using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class TrackingDocumentOrm : TrackingDocument
{
    protected TrackingDocumentOrm(IClock clock) : base(clock) { }
    private TrackingDocumentOrm() : base(GlobalClock.Clock) { }
    public ICollection<HashOfDocumentOrm>? HashOfDocuments { get; private set; }
    public ICollection<SignatureOrm>? Signatures { get; set; }
}