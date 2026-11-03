using System.Collections.Immutable;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Domain.Exceptions;
using SecureEDM.Common.Domain.Interfaces;
using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.AuthServer.Domain.Models;

public class UserSetting : IHasId<int>
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
    public TimeSpan ExpiresOfSessionIn
    {
        get; init;
    }
    public TimeSpan ExpiresOfRefreshTokenIn
    {
        get; init;
    }
    public TimeSpan ExpiresOfAccessTokenIn
    {
        get; init;
    }
    public TimeSpan ExpiresOfAttestationNoise
    {
        get; init;
    }
    protected UserSetting(IClock clock) { _clock = clock;}
}