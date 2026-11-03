using SecureEDM.Common.Domain.Models.Interfaces;

namespace SecureEDM.Common.Domain.Exceptions;

public class ExceptionDeletion(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionDeletion DeleteBeforeCreation { get; } = new("The deletion date cannot be earlier than the creation date.");
    public static ExceptionDeletion SoftDeletedAlreadyDo { get; } = new("The soft delete has already been done.");
    public static void CallDeleteBeforeCreation<TEntity>(TEntity entity) where TEntity : IHasCreated, IHasSoftDelete
    {
        if (entity.DeleteAt > entity.CreatedAt) return;
        throw DeleteBeforeCreation;
    }
    public static void CallSoftDeletedAlreadyDo<TEntity>(TEntity entity) where TEntity : IHasSoftDelete
    {
        if (entity.DeleteAt == null) return;
        throw SoftDeletedAlreadyDo;
    }
}