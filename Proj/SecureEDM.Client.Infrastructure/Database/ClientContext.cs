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
        var binaryConverter = new ValueConverter<ImmutableArray<byte>, byte[]>(
            convertToProviderExpression: x => x.ToArray(),
            convertFromProviderExpression: x => x.ToImmutableArray() 
        );
        var binaryComparer = new ValueComparer<ImmutableArray<byte>>(
            equalsExpression: (x1, x2) => true,
            hashCodeExpression: x => x.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            snapshotExpression: x => x.ToImmutableArray()
        );
        configurationBuilder
            .Properties<ImmutableArray<byte>>()
                .HaveConversion<ImmutableArrayToArrayConverter<byte>, ImmutableArrayBitesComparer<byte>>();
    }
}