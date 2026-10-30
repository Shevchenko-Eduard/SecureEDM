using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Common.Domain.Models.ValueObject;

public static class GlobalClock
{
    public static IClock Clock
    {
        get
        {
            ExceptionNullableValue.CallValueIsNull(field);
            return field;
        }
        private set;
    } = null!;
    public static void SetClock(IClock clock) => Clock = clock;
}