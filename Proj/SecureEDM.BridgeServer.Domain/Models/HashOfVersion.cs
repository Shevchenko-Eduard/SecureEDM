using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Exceptions;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.BridgeServer.Domain.Models;

public class HashOfVersion : IHasId<int>, IHasCreated, IHasRevocable<EnumRevokedReason>
{
    protected readonly IClock _clock;

    public int Id
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public int VersionID
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public ImmutableArray<byte> HashedAlgorithm
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public int RemoteAuthorId
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public ImmutableArray<byte> HashValue
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public int? RevokedBy
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is int revokedby) ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(revokedby);
            field = value;
        }
    }
    public bool IsPrimary
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public DateTimeOffset CreatedAt
    {
        get; init
        {
            ExceptionInvalidDate.CallDateFromTheFuture( _clock, value);
            field = value;
        }
    }
    public DateTimeOffset? RevokedAt
    {
        get; private set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is DateTimeOffset revoked) ExceptionInvalidDate.CallDateFromTheFuture(_clock, revoked);
            ExceptionRevocation.CallRevokedBeforeCreation<HashOfVersion, EnumRevokedReason>(this);
            field = value;
        }
    }
    public EnumRevokedReason RevokedReason { get; private set;}
    protected HashOfVersion(IClock clock) { _clock = clock;}
    public void Revoke(EnumRevokedReason reason) => Revoke(reason, _clock.Now);

    public void Revoke(EnumRevokedReason reason, DateTimeOffset revokedAt)
    {
        ExceptionRevocation.CallAlreadyRevoked(this);
        ExceptionRevocation.CallInvalidReason(reason);
        RevokedReason = reason;
        RevokedAt = revokedAt;
    }
}