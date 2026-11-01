using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Database.ModelOrm;

public class HashOfDocumentOrm : HashOfDocument, IEntityTypeConfiguration<HashOfDocumentOrm>
{
    protected HashOfDocumentOrm(IClock clock) : base(clock) { }
    private HashOfDocumentOrm() : base(GlobalClock.Clock) { }
    public TrackingDocumentOrm? TrackingDocument { get; private set; }

    public void Configure(EntityTypeBuilder<HashOfDocumentOrm> builder)
    {
        builder.HasIndex(b => b.HashedAlgorithm);
        builder.HasIndex(b => b.CreatedAt);
    }
}