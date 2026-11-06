using System.Security.Cryptography;
using SecureEDM.Client.Application.Interfaces;
using SecureEDM.Client.Application.Models;
using SecureEDM.Common.Infrastructure.Cryptography;
using SecureEDM.Common.Infrastructure.Cryptography.Enums;
using SecureEDM.Common.Infrastructure.Cryptography.Interfaces;
using SecureEDM.Common.Infrastructure.Cryptography.ValueObject;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Infrastructure.Cryptography;

/// <summary>Криптографический адаптер клиента поверх BouncyCastle-сервисов Common.</summary>
public sealed class ClientCrypto(
    IDigestAlgorithm? digest = null,
    ISignedAlgorithm? signed = null,
    IKpgAlgorithm? kpg = null) : IClientCrypto
{
    private readonly IDigestAlgorithm _digest = digest ?? new Digest();
    private readonly ISignedAlgorithm _signed = signed ?? new Signed();
    private readonly IKpgAlgorithm _kpg = kpg ?? new Kpg();

    public async ValueTask<GeneratedKeyPair> GenerateKeyPairAsync(
        EnumKpgAlgorithm algorithm, int keyLength, CancellationToken token = default)
    {
        var pair = await _kpg.Generate(algorithm, keyLength, token);

        var publicKey = pair.PublicKey.ToArray();
        var privateKey = pair.PrivateKey.ToArray();
        var fingerprint = await _digest.Hashing(
            EnumDigestAlgorithm.SHA_256,
            new MemoryStream(publicKey),
            token: token);

        return new GeneratedKeyPair(
            Convert.ToBase64String(publicKey),
            Convert.ToBase64String(privateKey),
            FormatFingerprint(fingerprint));
    }

    public ValueTask<string> ComputeSha256Async(string text, CancellationToken token = default) =>
        ComputeHashAsync(EnumDigestAlgorithm.SHA_256, text, token);

    public async ValueTask<string> ComputeHashAsync(
        EnumDigestAlgorithm algorithm, string text, CancellationToken token = default)
    {
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        var hash = await _digest.Hashing(algorithm, stream, token: token);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public async ValueTask<byte[]> SignAsync(
        EnumSignedAlgorithm algorithm, string content, byte[] privateKey, CancellationToken token = default)
    {
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        return await _signed.Signing(algorithm, stream, privateKey, token: token);
    }

    public async ValueTask<bool> VerifyAsync(
        EnumSignedAlgorithm algorithm, string content, byte[] signature, byte[] publicKey,
        CancellationToken token = default)
    {
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        return await _signed.Verify(
            new Signature(algorithm, signature, publicKey),
            stream,
            token: token);
    }

    public ValueTask<bool> VerifyPrehashedAsync(
        EdoKeyAlgorithm algorithm,
        byte[] hash,
        byte[] signature,
        byte[] publicKey,
        bool ieeeP1363,
        CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            if (algorithm == EdoKeyAlgorithm.Rsa)
            {
                using var rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
                return ValueTask.FromResult(rsa.VerifyHash(
                    hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
            }

            using var ecdsa = ECDsa.Create();
            ecdsa.ImportSubjectPublicKeyInfo(publicKey, out _);
            return ValueTask.FromResult(ecdsa.VerifyHash(
                hash,
                signature,
                ieeeP1363
                    ? DSASignatureFormat.IeeeP1363FixedFieldConcatenation
                    : DSASignatureFormat.Rfc3279DerSequence));
        }
        catch (CryptographicException)
        {
            return ValueTask.FromResult(false);
        }
        catch (ArgumentException)
        {
            return ValueTask.FromResult(false);
        }
    }

    public IReadOnlyList<EnumSignedAlgorithm> GetCompatibleSignatureAlgorithms(
        EnumKpgAlgorithm algorithm)
    {
        if (algorithm is EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS)
        {
            return Enum.GetValues<EnumSignedAlgorithm>()
                .Where(a => a is not (EnumSignedAlgorithm.RSA or EnumSignedAlgorithm.RAWRSASSA_PSS) &&
                            a.ToString().Contains("RSA", StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        if (!EdoAlgorithms.SupportsSigning(algorithm))
            return [];

        try
        {
            return CompatibilityTable.Table.GetCompatibles(algorithm);
        }
        catch (KeyNotFoundException)
        {
            return EdoAlgorithms.CompatibleSignatures(algorithm);
        }
    }

    public EnumSignedAlgorithm? GetDefaultSignatureAlgorithm(EnumKpgAlgorithm algorithm) =>
        EdoAlgorithms.DefaultSignatureAlgorithm(algorithm);

    private static string FormatFingerprint(byte[] fingerprint) =>
        string.Join(':', fingerprint.Select(b => b.ToString("x2")));
}
