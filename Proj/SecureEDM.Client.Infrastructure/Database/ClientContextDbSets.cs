using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;

namespace SecureEDM.Client.Infrastructure.Database;

public partial class ClientContext : DbContext
{
    public DbSet<AsymmetricKeyOrm> AsymmetricKeys { get; set; }
    public DbSet<HashOfDocumentOrm> HashOfDocuments { get; set; }
    public DbSet<PrivateKeyOrm> PrivateKeys { get; set; }
    public DbSet<PublicKeyOrm> PublicKeys { get; set; }
    public DbSet<SignatureOrm> Signatures { get; set; }
    public DbSet<TrackingDocumentOrm> TrackingDocuments { get; set; }
}