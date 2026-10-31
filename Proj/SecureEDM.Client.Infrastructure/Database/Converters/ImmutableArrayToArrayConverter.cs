using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SecureEDM.Client.Infrastructure.Database.Converters;

internal class ImmutableArrayToArrayConverter<T>() : ValueConverter<ImmutableArray<T>, T[]>(
        convertToProviderExpression: x => x.ToArray(),
        convertFromProviderExpression: x => x.ToImmutableArray())
{ }
