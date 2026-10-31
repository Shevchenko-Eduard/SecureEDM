using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecureEDM.Client.Infrastructure.Database.Comparers;
using SecureEDM.Client.Infrastructure.Database.Converters;

namespace SecureEDM.Client.Infrastructure.Database;

public partial class ClientContext : DbContext
{
    public ClientContext() : base() { }
    public ClientContext(DbContextOptions<ClientContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<ImmutableArray<byte>>()
            .HaveConversion<ImmutableArrayToArrayConverter<byte>, ImmutableArrayBitesComparer<byte>>();
    }
}