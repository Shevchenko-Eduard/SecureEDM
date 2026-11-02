using Mapster;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Common.Infrastructure;

public partial class MapperService : IMapper
{
    public TProvider Map<TSource, TProvider>(TSource source) where TSource : notnull where TProvider : notnull => source.Adapt<TProvider>();
}