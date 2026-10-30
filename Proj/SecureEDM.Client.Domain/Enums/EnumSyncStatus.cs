namespace SecureEDM.Client.Domain.Enums;

public enum EnumSyncStatus
{
    Pending,
    Synced,
    Conflict,
    DeleteConflict,
    DeleteLocally,
    DeleteRemotely,
    InsufficientRights,
    Detached,
    SyncError
}