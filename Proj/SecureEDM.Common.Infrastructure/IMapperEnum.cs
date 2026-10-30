using System.Collections.ObjectModel;

namespace SecureEDM.Common.Infrastructure;

public abstract class IMapperEnum<TEnum> where TEnum : struct, Enum
{
    protected static ReadOnlyDictionary<TEnum, string> IdToName;
    protected static readonly ReadOnlyDictionary<string, TEnum> nameToId = new(new Dictionary<string, TEnum>(IdToName?.Select(s => new KeyValuePair<string, TEnum>(s.Value, s.Key)) ?? throw new ArgumentNullException()));
    public static string TryFromID(TEnum id)
    {
        IdToName.TryGetValue(id, out var res);
        if (res == null) throw new ArgumentNullException();
        return res;
    }
    public static TEnum TryFromName(string name)
    {
        nameToId.TryGetValue(name, out var res);
        return res;
    }
}