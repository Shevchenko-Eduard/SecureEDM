namespace SecureEDM.Common.Domain.Models.Interfaces;

public interface IHasCreated
{
    public DateTimeOffset CreatedAt { get; init; }
}