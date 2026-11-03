using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Exceptions;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.BridgeServer.Domain.Models;

public class UserDocumentLnk: IHasCreated
{
    protected readonly IClock _clock;

    public int DocumentID
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public int UserID
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public int RightsID
    {
        get; init
        {
            ExceptionInvalidId.CallIdIsLessThanOrEqualToZero(value);
            field = value;
        }
    }
    public bool IsOwner
    {
        get; init
        {
            ExceptionNullableValue.CallValueIsNull(value);
            field = value;
        }
    }
    public int GrantedBy
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
    protected UserDocumentLnk(IClock clock) { _clock = clock; }
}