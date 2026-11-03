using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Exceptions;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.AuthServer.Domain.Models;

public class User : IHasId<int>, IHasCreated, IHasSoftDelete
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
    public string Name
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    } = null!;
    public string? Email
    {
        get; private set
        {
            ExceptionNullableValue.CallValueIsNull(value!);
            field = value;
        }
    } = null!;
    public DateOnly? DOB
    {
        get; init
        {
            ExceptionInvalidDate.CallDateFromTheFuture(_clock, new(new DateTime((DateOnly)value!, default)));
            field = value;
        }
    }
    public int SettingID
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
            ExceptionInvalidDate.CallDateFromTheFuture( _clock, value);
            field = value;
        }
    }
    public DateTimeOffset? UpdatedAt
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is DateTimeOffset Updated) ExceptionInvalidDate.CallDateFromTheFuture(_clock, Updated);
            field = value;
        }
    }
    public DateTimeOffset? DeleteAt
    {
        get; private set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is DateTimeOffset deleted) ExceptionInvalidDate.CallDateFromTheFuture(_clock, deleted);
            ExceptionDeletion.CallDeleteBeforeCreation(this);
            field = value;
        }
    }


    protected User(IClock clock) { _clock = clock; }

    public void SoftDelete() => SoftDelete(_clock.Now);

    public void SoftDelete(DateTimeOffset deletedAt)
    {
        ExceptionDeletion.CallSoftDeletedAlreadyDo(this);
        DeleteAt = deletedAt;
    }
}