using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Client.Domain.Models;

public class AsymmetricKey : IHasId<int>, IHasRevocable<EnumRevokedReason>, IHasCreated
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
            ExceptionRevocation.CallRevokedBeforeCreation<AsymmetricKey, EnumRevokedReason>(this);
            field = value;
        }
    }
    public EnumRevokedReason RevokedReason { get; private set; }
    public EnumPairKeygenAlgorithm KeygenAlgorithm { get; init; }

    public AsymmetricKey(
        IClock clock,
        EnumPairKeygenAlgorithm keygenAlgorithm
    )
    {
        _clock = clock;
        KeygenAlgorithm = keygenAlgorithm;
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