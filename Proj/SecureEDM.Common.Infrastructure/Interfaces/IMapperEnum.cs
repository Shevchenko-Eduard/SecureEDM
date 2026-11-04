using System.Collections.ObjectModel;

namespace SecureEDM.Common.Infrastructure.Interfaces;

public abstract class IMapperEnum<TEnum> where TEnum : struct, Enum
{
    public readonly ReadOnlyDictionary<TEnum, string> IdToName;
    // private readonly ReadOnlyDictionary<string, TEnum> _nameToId;
    protected IMapperEnum(Dictionary<TEnum, string> mapping)
    {
        ArgumentNullException.ThrowIfNull(mapping);
        IdToName = new(mapping);
        // _nameToId = new(mapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key));
    }
    public string TryFromID(TEnum id)
    {
        if (IdToName != null && IdToName.TryGetValue(id, out var res)) return res;
        throw new KeyNotFoundException(id.ToString());
    }
    // public TEnum TryFromName(string name)
    // {
    //     ArgumentNullException.ThrowIfNull(name);
    //     if (_nameToId.TryGetValue(name, out var res)) return res;
    //     throw new KeyNotFoundException(name);
    // }
    public TEnum[] GetIds() => [.. IdToName.Select(x => x.Key)];
    public string[] GetNames() => [.. IdToName.Select(x => x.Value)];
}