using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Application.Models;

public static class EdoLabels
{
    public static string HashAlgorithm(EnumDigestAlgorithm algorithm) => algorithm switch
    {
        EnumDigestAlgorithm.SHA_256 => "SHA-256",
        EnumDigestAlgorithm.SHA_512 => "SHA-512",
        EnumDigestAlgorithm.SHA3_256 => "SHA3-256",
        EnumDigestAlgorithm.SHA3_512 => "SHA3-512",
        EnumDigestAlgorithm.GOST3411_2012_256 => "GOST3411-2012-256",
        EnumDigestAlgorithm.GOST3411_2012_512 => "GOST3411-2012-512",
        EnumDigestAlgorithm.BLAKE2B_256 => "BLAKE2B-256",
        _ => algorithm.ToString().Replace('_', '-'),
    };

    public static string RevokedReason(EnumRevokedReason reason) => reason switch
    {
        EnumRevokedReason.Outdated => "Устарел",
        EnumRevokedReason.Cancelled => "Отменён",
        EnumRevokedReason.Compromise => "Компрометация",
        _ => "Не указана",
    };

    public static string ConflictStrategy(EnumConflictStrategy strategy) => strategy switch
    {
        EnumConflictStrategy.KeepLocal => "Оставить локальную версию",
        EnumConflictStrategy.KeepRemote => "Оставить удалённую версию",
        EnumConflictStrategy.KeepRemoteInNewLocal => "Удалённую — в новый локальный файл",
        EnumConflictStrategy.KeepLocalInNewRemote => "Локальную — в новую удалённую версию",
        _ => strategy.ToString(),
    };
}
