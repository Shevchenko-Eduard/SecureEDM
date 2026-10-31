using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Client.Domain.Models;

public class TrackingDocument : IHasId<int>, IHasCreated, IHasSoftDelete
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
    public string Path
    {
        get; set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    } = null!;
    public EnumStatusTracking? StatusTracking
    {
        get; set
        {
            ExceptionNullableValue.CallValueIsNull(value);
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
    public DateTimeOffset? LastSync
    {
        get; set
        {
            ExceptionNullableValue.CallValueIsNull(value);
            if (value is DateTimeOffset lastSync) ExceptionInvalidDate.CallDateFromTheFuture(_clock, lastSync);
            field = value;
        }
    }
    public string FixedExtension
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    } = null!;
    // public TrackingDocument(IClock clock, string path, string extension)
    // {
    //     _clock = clock;
    //     Path = path;
    //     CreatedAt = _clock.Now;
    //     FixedExtension = extension;
    // }
    protected TrackingDocument(IClock clock) { _clock = clock; }
}