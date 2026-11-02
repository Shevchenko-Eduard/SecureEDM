namespace SecureEDM.Common.Contracts.Interfaces;

public interface IMapper
{
    public TProvider Map<TSource, TProvider>(TSource source) where TSource : notnull where TProvider : notnull;
}