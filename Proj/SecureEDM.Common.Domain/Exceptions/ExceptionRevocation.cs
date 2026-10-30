using System.Numerics;
using SecureEDM.Common.Domain.Models.Interfaces;

public class ExceptionRevocation(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    public static ExceptionRevocation AlreadyRevoked { get; } = new("Cannot be revoked again.");
    public static ExceptionRevocation InvalidReason { get; } = new("Not a valid reason.");
    public static ExceptionRevocation RevokedBeforeCreation { get; } = new("The revoked date cannot be earlier than the creation date.");
    public static void CallAlreadyRevoked<TReason>(IHasRevocable<TReason> revocable) where TReason : Enum
    {
        if (revocable.RevokedAt is null && revocable.RevokedReason is null) return;
        throw AlreadyRevoked;
    }
    public static void CallInvalidReason<TReason>(TReason reason) where TReason : struct, Enum
    {
        if (Enum.IsDefined(reason)) return;
        throw InvalidReason;
    }
    public static void CallRevokedBeforeCreation<TEntity, TReason>(TEntity entity) where TReason : Enum where TEntity: IHasCreated, IHasRevocable<TReason>
    {
        if (entity.RevokedAt > entity.CreatedAt) return;
        throw RevokedBeforeCreation;
    }
}