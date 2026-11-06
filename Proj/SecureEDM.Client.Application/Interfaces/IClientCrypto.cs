using SecureEDM.Client.Application.Models;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Application.Interfaces;

public sealed record GeneratedKeyPair(
    string PublicKeyBase64,
    string PrivateKeyBase64,
    string Fingerprint);

public interface IClientCrypto
{
    ValueTask<GeneratedKeyPair> GenerateKeyPairAsync(
        EnumKpgAlgorithm algorithm,
        int keyLength,
        CancellationToken token = default);

    IReadOnlyList<EnumSignedAlgorithm> GetCompatibleSignatureAlgorithms(EnumKpgAlgorithm algorithm);

    EnumSignedAlgorithm? GetDefaultSignatureAlgorithm(EnumKpgAlgorithm algorithm);

    ValueTask<string> ComputeSha256Async(string text, CancellationToken token = default);

    ValueTask<string> ComputeHashAsync(
        EnumDigestAlgorithm algorithm,
        string text,
        CancellationToken token = default);

    ValueTask<byte[]> SignAsync(
        EnumSignedAlgorithm algorithm,
        string content,
        byte[] privateKey,
        CancellationToken token = default);

    ValueTask<bool> VerifyAsync(
        EnumSignedAlgorithm algorithm,
        string content,
        byte[] signature,
        byte[] publicKey,
        CancellationToken token = default);

    /// <summary>Проверка подписи, созданной по готовому дайджесту (формат Web Crypto/HTML-прототипа).</summary>
    ValueTask<bool> VerifyPrehashedAsync(
        EdoKeyAlgorithm algorithm,
        byte[] hash,
        byte[] signature,
        byte[] publicKey,
        bool ieeeP1363,
        CancellationToken token = default);
}
