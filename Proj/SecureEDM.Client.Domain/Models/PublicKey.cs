using System.Collections.Immutable;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Client.Domain.Models;

public class PublicKey : IHasId<int>
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
    public ImmutableArray<byte>? Attestation
    {
        get; private set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public int? RemoteOwnerId
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is int id) ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(id);
            field = value;
        }
    }
    public PublicKey(
        IClock clock,
        int asymmetricKeyId,
        byte[] keyValue,
        byte[]? attestation = null,
        int? remoteOwnerId = null
    )
    {
        _clock = clock;
        AsymmetricKeyId = asymmetricKeyId;
        KeyValue = [.. keyValue];
        if (attestation != null) Attestation = [.. attestation];
        RemoteOwnerId = remoteOwnerId;
    }
}