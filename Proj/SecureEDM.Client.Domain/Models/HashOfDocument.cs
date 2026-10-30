using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Client.Domain.Models;

public class HashOfDocument : IHasId<int>, IHasCreated, IHasRevocable<EnumRevokedReason>
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
    public DateTimeOffset CreatedAt
    {
        get; init
        {
            ExceptionInvalidDate.CallDateFromTheFuture(_clock, value);
            field = value;
        }
    }
    public DateTimeOffset? RevokedAt
    {
        get; private set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is DateTimeOffset revoked) ExceptionInvalidDate.CallDateFromTheFuture(_clock, revoked);
            ExceptionRevocation.CallRevokedBeforeCreation<HashOfDocument, EnumRevokedReason>(this);
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
    public int? RemoteAuthorId
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is int id) ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(id);
            field = value;
        }
    }
    public EnumDigestAlgorithm HashedAlgorithm { get; init; }
    public ImmutableArray<byte> Hash
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public bool IsPrimary { get; private set; }
    public HashOfDocument(
        IClock clock, byte[] hash, int documentId,
        EnumDigestAlgorithm algorithm,
        int? remoteAuthorId = null, bool isPrimary = false)
    {
        _clock = clock;
        Hash = [.. hash];
        DocumentId = documentId;
        HashedAlgorithm = algorithm;
        RemoteAuthorId = remoteAuthorId;
        IsPrimary = isPrimary;
        CreatedAt = _clock.Now;
    }

    public void Revoke(EnumRevokedReason reason)
    {
        ExceptionRevocation.CallAlreadyRevoked(this);
        ExceptionRevocation.CallInvalidReason(reason);
        RevokedReason = reason;
        RevokedAt = _clock.Now;
    }
}