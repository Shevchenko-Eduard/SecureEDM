using System.Collections.ObjectModel;
using System.Dynamic;

namespace SecureEDM.Common.Infrastructure.Interfaces;

public abstract class IMapperEnum<TEnum> where TEnum : struct, Enum
{
    private readonly ReadOnlyDictionary<TEnum, string> _idToName;
    private readonly ReadOnlyDictionary<string, TEnum> _nameToId;
    protected IMapperEnum(Dictionary<TEnum, string> mapping)
    {
        ArgumentNullException.ThrowIfNull(mapping);
        _idToName = new(mapping);
        _nameToId = new(mapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key));
    }
    public string TryFromID(TEnum id)
    {
        _ = _nameToId;
        if (_idToName != null && _idToName.TryGetValue(id, out var res)) return res;
        throw new KeyNotFoundException(id.ToString());
    }
    public TEnum TryFromName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (_nameToId.TryGetValue(name, out var res)) return res;
        throw new KeyNotFoundException(name);
    }
}