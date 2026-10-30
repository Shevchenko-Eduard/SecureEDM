using SecureEDM.Common.Domain.Models.Interfaces;

public class ExceptionDeletion(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionDeletion DeleteBeforeCreation { get; } = new("The deletion date cannot be earlier than the creation date.");
    public static void CallDeleteBeforeCreation<TEntity>(TEntity entity) where TEntity: IHasCreated, IHasSoftDelete
    {
        if (entity.DeleteAt > entity.CreatedAt) return;
        throw DeleteBeforeCreation;
    }
}