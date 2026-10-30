using System.Collections.Immutable;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Client.Domain.Models;

public class PrivateKey : IHasId<int>
{
    private readonly IClock _clock;
    public int Id
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public int AsymmetricKeyId
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public ImmutableArray<byte> KeyValue
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public PrivateKey(
        IClock clock,
        int asymmetricKeyId,
        byte[] keyValue
    )
    {
        _clock = clock;
        AsymmetricKeyId = asymmetricKeyId;
        KeyValue = [.. keyValue];
    }
}