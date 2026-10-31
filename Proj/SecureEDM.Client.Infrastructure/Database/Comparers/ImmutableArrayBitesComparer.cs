using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SecureEDM.Client.Infrastructure.Database.Comparers;

internal class ImmutableArrayBitesComparer<T>() : ValueComparer<ImmutableArray<T>>(
    equalsExpression: (c1, c2) => c1.SequenceEqual(c2),
        hashCodeExpression: x => x.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        snapshotExpression: x => x.ToImmutableArray()) where T : notnull
{ }