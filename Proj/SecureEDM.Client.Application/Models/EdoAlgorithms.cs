using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Application.Models;

/// <summary>Сопоставление алгоритмов реестра с криптопримитивами Common.</summary>
public static class EdoAlgorithms
{
    public static readonly IReadOnlyList<EnumKpgAlgorithm> KeyAlgorithms =
    [
        EnumKpgAlgorithm.RSA,
        EnumKpgAlgorithm.RSASSA_PSS,
        EnumKpgAlgorithm.ECDSA,
        EnumKpgAlgorithm.EC,
        EnumKpgAlgorithm.ECDH,
        EnumKpgAlgorithm.ECDHC,
        EnumKpgAlgorithm.ECMQV,
        EnumKpgAlgorithm.Ed25519,
        EnumKpgAlgorithm.Ed448,
        EnumKpgAlgorithm.DSA,
        EnumKpgAlgorithm.GOST3410,
        EnumKpgAlgorithm.ML_DSA_44,
        EnumKpgAlgorithm.ML_DSA_65,
        EnumKpgAlgorithm.ML_DSA_87,
        EnumKpgAlgorithm.ML_KEM,
        EnumKpgAlgorithm.SLH_DSA,
        EnumKpgAlgorithm.X25519,
        EnumKpgAlgorithm.X448,
        EnumKpgAlgorithm.DH,
        EnumKpgAlgorithm.ELGAMAL,
    ];

    public static EnumKpgAlgorithm ToKeygenAlgorithm(EdoKeyAlgorithm algorithm) =>
        algorithm == EdoKeyAlgorithm.Rsa ? EnumKpgAlgorithm.RSA : EnumKpgAlgorithm.EC;

    public static EdoKeyAlgorithm ToLegacy(EnumKpgAlgorithm algorithm) =>
        algorithm is EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA or EnumKpgAlgorithm.ECDH
            or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV
            ? EdoKeyAlgorithm.Ecdsa
            : EdoKeyAlgorithm.Rsa;

    public static int KeyLength(EdoKeyAlgorithm algorithm) =>
        algorithm == EdoKeyAlgorithm.Rsa ? 2048 : 256;

    public static EnumSignedAlgorithm ToSignedAlgorithm(EdoKeyAlgorithm algorithm) =>
        algorithm == EdoKeyAlgorithm.Rsa
            ? EnumSignedAlgorithm.SHA_256withRSAandMGF1
            : EnumSignedAlgorithm.SHA_256withECDSA;

    public static string Label(EdoKeyAlgorithm algorithm) =>
        algorithm == EdoKeyAlgorithm.Rsa ? "RSA-PSS 2048" : "ECDSA P-256";

    public static string SerializedName(EdoKeyAlgorithm algorithm) =>
        algorithm == EdoKeyAlgorithm.Rsa ? "RSA" : "ECDSA";

    public static EdoKeyAlgorithm Parse(string? value) =>
        value is not null && value.Contains("ECDSA", StringComparison.OrdinalIgnoreCase)
            ? EdoKeyAlgorithm.Ecdsa
            : EdoKeyAlgorithm.Rsa;

    public static string KeyLabel(EnumKpgAlgorithm algorithm) => algorithm switch
    {
        EnumKpgAlgorithm.RSA => "RSA",
        EnumKpgAlgorithm.RSASSA_PSS => "RSA-PSS",
        EnumKpgAlgorithm.DSA => "DSA",
        EnumKpgAlgorithm.EC => "EC",
        EnumKpgAlgorithm.ECDSA => "ECDSA",
        EnumKpgAlgorithm.ECDH => "ECDH",
        EnumKpgAlgorithm.ECDHC => "ECDHC",
        EnumKpgAlgorithm.ECMQV => "ECMQV",
        EnumKpgAlgorithm.Ed25519 => "Ed25519",
        EnumKpgAlgorithm.Ed448 => "Ed448",
        EnumKpgAlgorithm.GOST3410 => "GOST 34.10",
        EnumKpgAlgorithm.ML_DSA_44 => "ML-DSA-44",
        EnumKpgAlgorithm.ML_DSA_65 => "ML-DSA-65",
        EnumKpgAlgorithm.ML_DSA_87 => "ML-DSA-87",
        EnumKpgAlgorithm.ML_KEM => "ML-KEM",
        EnumKpgAlgorithm.SLH_DSA => "SLH-DSA",
        EnumKpgAlgorithm.X25519 => "X25519",
        EnumKpgAlgorithm.X448 => "X448",
        EnumKpgAlgorithm.DH => "DH",
        EnumKpgAlgorithm.ELGAMAL => "ElGamal",
        _ => algorithm.ToString().Replace('_', '-'),
    };

    public static string KeyDescription(EnumKpgAlgorithm algorithm) => algorithm switch
    {
        EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS => "Подпись RSA / RSA-PSS",
        EnumKpgAlgorithm.DSA => "Подпись DSA",
        EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA or EnumKpgAlgorithm.ECDH
            or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV => "Подпись ECDSA на эллиптических кривых",
        EnumKpgAlgorithm.Ed25519 or EnumKpgAlgorithm.Ed448 => "Подпись EdDSA",
        EnumKpgAlgorithm.GOST3410 => "Подпись ГОСТ 34.10",
        EnumKpgAlgorithm.ML_DSA_44 or EnumKpgAlgorithm.ML_DSA_65 or EnumKpgAlgorithm.ML_DSA_87
            => "Постквантовый ML-DSA (ключи без подписи в текущем API)",
        EnumKpgAlgorithm.ML_KEM => "Постквантовый ML-KEM (обмен ключами, без подписи)",
        EnumKpgAlgorithm.SLH_DSA => "Постквантовый SLH-DSA (ключи без подписи в текущем API)",
        EnumKpgAlgorithm.X25519 or EnumKpgAlgorithm.X448 => "Обмен ключами (без подписи)",
        EnumKpgAlgorithm.DH or EnumKpgAlgorithm.ELGAMAL => "Шифрование / обмен ключами (без подписи)",
        _ => "Криптографический алгоритм",
    };

    public static IReadOnlyList<int> KeyLengths(EnumKpgAlgorithm algorithm) => algorithm switch
    {
        EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS => [2048, 3072, 4096],
        EnumKpgAlgorithm.DSA => [1024, 2048, 3072],
        EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA or EnumKpgAlgorithm.ECDH
            or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV => [256, 384, 521],
        EnumKpgAlgorithm.Ed25519 => [255],
        EnumKpgAlgorithm.Ed448 => [448],
        EnumKpgAlgorithm.GOST3410 => [256],
        EnumKpgAlgorithm.ML_DSA_44 => [44],
        EnumKpgAlgorithm.ML_DSA_65 => [65],
        EnumKpgAlgorithm.ML_DSA_87 => [87],
        EnumKpgAlgorithm.ML_KEM => [512, 768, 1024],
        EnumKpgAlgorithm.SLH_DSA => [128, 192, 256],
        EnumKpgAlgorithm.X25519 => [255],
        EnumKpgAlgorithm.X448 => [448],
        EnumKpgAlgorithm.DH or EnumKpgAlgorithm.ELGAMAL => [512, 1024, 2048],
        _ => [2048],
    };

    public static int DefaultKeyLength(EnumKpgAlgorithm algorithm) => KeyLengths(algorithm)[0];

    public static bool SupportsSigning(EnumKpgAlgorithm algorithm) =>
        algorithm is EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS
            or EnumKpgAlgorithm.DSA or EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA
            or EnumKpgAlgorithm.ECDH or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV
            or EnumKpgAlgorithm.Ed25519 or EnumKpgAlgorithm.Ed448 or EnumKpgAlgorithm.GOST3410;

    public static string SignatureLabel(EnumSignedAlgorithm algorithm) =>
        algorithm.ToString().Replace('_', '-');

    public static string KeyAlgorithmLabel(EdoKey key) =>
        KeyLabel(KeyAlgorithm(key));

    public static EnumKpgAlgorithm KeyAlgorithm(EdoKey key) =>
        key.KeygenAlgorithm ?? ToKeygenAlgorithm(key.Algorithm);

    public static string SignatureAlgorithmLabel(EdoSignature signature) =>
        SignatureLabel(SignedAlgorithm(signature));

    public static EnumSignedAlgorithm SignedAlgorithm(EdoSignature signature) =>
        signature.SignedAlgorithm ?? ToSignedAlgorithm(signature.Algorithm);

    public static EnumSignedAlgorithm? DefaultSignatureAlgorithm(EnumKpgAlgorithm algorithm) => algorithm switch
    {
        EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS => EnumSignedAlgorithm.SHA_256withRSAandMGF1,
        EnumKpgAlgorithm.DSA => EnumSignedAlgorithm.SHA_256withDSA,
        EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA or EnumKpgAlgorithm.ECDH
            or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV => EnumSignedAlgorithm.SHA_256withECDSA,
        EnumKpgAlgorithm.Ed25519 => EnumSignedAlgorithm.Ed25519,
        EnumKpgAlgorithm.Ed448 => EnumSignedAlgorithm.Ed448,
        EnumKpgAlgorithm.GOST3410 => EnumSignedAlgorithm.GOST3410,
        _ => null,
    };

    public static IReadOnlyList<EnumSignedAlgorithm> CompatibleSignatures(EnumKpgAlgorithm algorithm)
    {
        if (!SupportsSigning(algorithm)) return [];

        if (algorithm is EnumKpgAlgorithm.RSA or EnumKpgAlgorithm.RSASSA_PSS)
            return Enum.GetValues<EnumSignedAlgorithm>()
                .Where(a => a is not (EnumSignedAlgorithm.RSA or EnumSignedAlgorithm.RAWRSASSA_PSS) &&
                            a.ToString().Contains("RSA", StringComparison.OrdinalIgnoreCase))
                .ToArray();

        if (algorithm == EnumKpgAlgorithm.DSA)
            return Enum.GetValues<EnumSignedAlgorithm>()
                .Where(a => a.ToString().Contains("DSA", StringComparison.OrdinalIgnoreCase))
                .ToArray();

        if (algorithm is EnumKpgAlgorithm.EC or EnumKpgAlgorithm.ECDSA or EnumKpgAlgorithm.ECDH
            or EnumKpgAlgorithm.ECDHC or EnumKpgAlgorithm.ECMQV)
            return Enum.GetValues<EnumSignedAlgorithm>()
                .Where(a =>
                    a.ToString().Contains("ECDSA", StringComparison.OrdinalIgnoreCase) ||
                    a.ToString().Contains("ECNR", StringComparison.OrdinalIgnoreCase) ||
                    a.ToString().Contains("SM2", StringComparison.OrdinalIgnoreCase) ||
                    a.ToString().Contains("ECGOST", StringComparison.OrdinalIgnoreCase))
                .ToArray();

        if (algorithm == EnumKpgAlgorithm.Ed25519)
            return [EnumSignedAlgorithm.Ed25519, EnumSignedAlgorithm.Ed25519ctx, EnumSignedAlgorithm.Ed25519ph];

        if (algorithm == EnumKpgAlgorithm.Ed448)
            return [EnumSignedAlgorithm.Ed448, EnumSignedAlgorithm.Ed448ph];

        if (algorithm == EnumKpgAlgorithm.GOST3410)
            return [EnumSignedAlgorithm.GOST3410];

        return [];
    }

    public static bool AreCompatible(EnumKpgAlgorithm keygen, EnumSignedAlgorithm signature) =>
        CompatibleSignatures(keygen).Contains(signature);

    public static EnumSignedAlgorithm? TryParseSignatureAlgorithm(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        // В HTML-прототипе поле algorithm содержит имя семейства ключа, а не алгоритм подписи.
        if (value.Equals("RSA", StringComparison.OrdinalIgnoreCase))
            return EnumSignedAlgorithm.SHA_256withRSAandMGF1;
        if (value.Equals("ECDSA", StringComparison.OrdinalIgnoreCase))
            return EnumSignedAlgorithm.SHA_256withECDSA;

        if (Enum.TryParse<EnumSignedAlgorithm>(value, true, out var parsed)) return parsed;

        if (value.Contains("RSA", StringComparison.OrdinalIgnoreCase))
            return EnumSignedAlgorithm.SHA_256withRSAandMGF1;
        if (value.Contains("ECDSA", StringComparison.OrdinalIgnoreCase))
            return EnumSignedAlgorithm.SHA_256withECDSA;

        return null;
    }

    public static EnumKpgAlgorithm? TryParseKeyAlgorithm(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (Enum.TryParse<EnumKpgAlgorithm>(value, true, out var parsed)) return parsed;

        if (value.Contains("ECDSA", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.ECDSA;
        if (value.Contains("RSA", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.RSA;
        if (value.Contains("Ed25519", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.Ed25519;
        if (value.Contains("Ed448", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.Ed448;

        return null;
    }

    public static EnumKpgAlgorithm? KeygenFromSignature(EnumSignedAlgorithm algorithm)
    {
        var name = algorithm.ToString();
        if (name.Contains("RSA", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.RSA;
        if (name.Contains("DSA", StringComparison.OrdinalIgnoreCase) &&
            !name.Contains("ECDSA", StringComparison.OrdinalIgnoreCase))
            return EnumKpgAlgorithm.DSA;
        if (name.Contains("ECDSA", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("ECNR", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("SM2", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("ECGOST", StringComparison.OrdinalIgnoreCase))
            return EnumKpgAlgorithm.EC;
        if (name.StartsWith("Ed25519", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.Ed25519;
        if (name.StartsWith("Ed448", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.Ed448;
        if (name.Contains("GOST", StringComparison.OrdinalIgnoreCase)) return EnumKpgAlgorithm.GOST3410;
        return null;
    }
}
