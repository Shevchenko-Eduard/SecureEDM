using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Exceptions;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.BridgeServer.Domain.Models;

public class Document : IHasId<int>, IHasCreated, IHasSoftDelete
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
    public string? Description
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value!);
            field = value;
        }
    } = null!;
    public string Extension
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    } = null!;
    public string MimeType
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    } = null!;
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


    protected Document(IClock clock) { _clock = clock; }

    public void SoftDelete() => SoftDelete(_clock.Now);

    public void SoftDelete(DateTimeOffset deletedAt)
    {
        ExceptionDeletion.CallSoftDeletedAlreadyDo(this);
        DeleteAt = deletedAt;
    }
}