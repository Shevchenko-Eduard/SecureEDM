namespace SecureEDM.Client.Application.Models;

/// <summary>Результат проверки подписи из реестра или из пакета.</summary>
public sealed class VerificationResult
{
    public bool SignatureOk { get; init; }
    public bool HashMatch { get; init; }
    public bool KeyOk { get; init; }
    public bool Revoked { get; init; }
    public bool Expired { get; init; }
    public bool SignatureRevoked { get; init; }

    public string VerdictKind { get; init; } = "bad";
    public string VerdictText { get; init; } = "Проверка не пройдена";
    public string Owner { get; init; } = "—";
    public string AlgorithmLabel { get; init; } = "—";
    public DateTimeOffset Time { get; init; }
    public string Hash { get; init; } = string.Empty;

    public bool IsValid => SignatureOk && HashMatch && KeyOk && !SignatureRevoked;
}
