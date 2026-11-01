using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class SignatureOrm : Signature, IEntityTypeConfiguration<SignatureOrm>
{
    protected SignatureOrm(IClock clock) : base(clock) { }
    private SignatureOrm() : base(GlobalClock.Clock) { }
    public TrackingDocumentOrm? TrackingDocument { get; private set; }
    public PublicKeyOrm? PublicKey { get; private set; }

    public void Configure(EntityTypeBuilder<SignatureOrm> builder)
    {
        builder.HasIndex(b => b.SignedAt);
        builder.HasIndex(b => b.SignedAlgorithm);
    }
}