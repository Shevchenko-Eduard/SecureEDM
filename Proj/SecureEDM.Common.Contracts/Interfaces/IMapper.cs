namespace SecureEDM.Common.Contracts.Interfaces;

public interface IMapper
{
    public IMapper<TProvider> To<TProvider>() where TProvider : notnull;
}

public interface IMapper<TProvider> where TProvider : notnull
{
    public TProvider From<TSource>(TSource source) where TSource : notnull;
    public TProvider From<TSource>(TSource source, out TProvider entity) where TSource : notnull;
}