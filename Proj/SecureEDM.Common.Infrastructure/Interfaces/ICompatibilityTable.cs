using System.Collections.Immutable;
using SecureEDM.Common.Infrastructure.Cryptography.ValueObject;

namespace SecureEDM.Common.Infrastructure.Interfaces;

public abstract class CompatibilityTable<TLeft, TRight>
    where TLeft : struct
    where TRight : struct
{
    private readonly ImmutableArray<CompatibleCouple<TLeft, TRight>> _compatibles;
    private readonly ImmutableDictionary<TLeft, TRight[]> _compatiblesLeft;
    private readonly ImmutableDictionary<TRight, TLeft[]> _compatiblesRight;
    protected CompatibilityTable(CompatibleCouple<TLeft, TRight>[] compatibles)
    {
        _compatibles = [.. compatibles];
        Dictionary<TLeft, List<TRight>> compatiblesLeft = [];
        Dictionary<TRight, List<TLeft>> compatiblesRight = [];
        foreach (var c in _compatibles)
        {
            if (!compatiblesLeft.TryGetValue(c.Left, out var leftList))
            {
                leftList = [];
                compatiblesLeft[c.Left] = leftList;
            }
            leftList.Add(c.Right);

            if (!compatiblesRight.TryGetValue(c.Right, out var rightList))
            {
                rightList = [];
                compatiblesRight[c.Right] = rightList;
            }
            rightList.Add(c.Left);
        }
        _compatiblesLeft = compatiblesLeft.ToImmutableDictionary(x => x.Key, x => x.Value.ToArray());
        _compatiblesRight = compatiblesRight.ToImmutableDictionary(x => x.Key, x => x.Value.ToArray());
    }
    public TLeft[] GetCompatibles(TRight value) => _compatiblesRight[value];
    public TRight[] GetCompatibles(TLeft value) => _compatiblesLeft[value];
}
