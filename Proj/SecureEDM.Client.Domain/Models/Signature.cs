using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Client.Domain.Models;

public class Signature : IHasId<int>, IHasRevocable<EnumRevokedReason>
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
    public DateTimeOffset? RevokedAt
    {
        get; private set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is DateTimeOffset revoked) ExceptionInvalidDate.CallDateFromTheFuture(_clock, revoked);
            field = value;
        }
    }
    public EnumRevokedReason RevokedReason { get; private set; }
    public int DocumentId
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public int PublicKeyId
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public ImmutableArray<byte> SignatureValue
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public DateTimeOffset SignedAt
    {
        get; init
        {
            ExceptionInvalidDate.CallDateFromTheFuture(_clock, value);
            field = value;
        }
    }
    public int? RemoteAuthorId
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is int id) ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(id);
            field = value;
        }
    }
    public EnumSignedAlgorithm SignedAlgorithm { get; init; }
    // public Signature(
    //     IClock clock, int documentId, int publicKeyId,
    //     byte[] signature, EnumSignedAlgorithm signedAlgorithm,
    //     DateTimeOffset signedAt, int? remoteAuthorId = null)
    // {
    //     _clock = clock;
    //     SignedAlgorithm = signedAlgorithm;
    //     DocumentId = documentId;
    //     PublicKeyId = publicKeyId;
    //     SignatureValue = [.. signature];
    //     SignedAt = signedAt;
    //     RemoteAuthorId = remoteAuthorId;
    // }
    protected Signature(IClock clock) { _clock = clock; }
    public void Revoke(EnumRevokedReason reason)
    {
        ExceptionRevocation.CallAlreadyRevoked(this);
        ExceptionRevocation.CallInvalidReason(reason);
        RevokedReason = reason;
        RevokedAt = _clock.Now;
    }
}