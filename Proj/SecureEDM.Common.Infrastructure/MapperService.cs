using Mapster;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Common.Infrastructure;

public class MapperService : IMapper
{
    public IMapper<TProvider> To<TProvider>() where TProvider : notnull => new MapperService<TProvider>();
}

public class MapperService<TProvider> : IMapper<TProvider> where TProvider : notnull
{
    public TProvider From<TSource>(TSource source) where TSource : notnull => source.Adapt<TProvider>();

    public TProvider From<TSource>(TSource source, out TProvider entity) where TSource : notnull
    {
        entity = From(source);
        return entity;
    }
}