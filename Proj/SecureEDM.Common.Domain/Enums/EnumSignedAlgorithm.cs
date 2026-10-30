namespace SecureEDM.Common.Domain.Enums;


public enum EnumSignedAlgorithm
{
    // BIP340 / BLS
    BIP340,
    BLS,

    // Обёртки / адаптеры
    DigestingMessage,
    DigestingStateAwareMessage,
    MessageSignerAdapter,
    Generic,

    // DSA / ECDSA
    DSADigest,
    ECCSI,

    // EdDSA
    Ed25519,
    Ed25519ctx,
    Ed25519ph,
    Ed448,
    Ed448ph,

    // Пост-квантовые
    MLDSA,
    HashMLDSA_44,
    HashMLDSA_65,
    HashMLDSA_87,
    SLHDSA,
    HashSLHDSA,

    // RSA
    PSS,
    RSADigest,
    ISO9796d2,
    ISO9796d2PSS,
    X931,

    // SM2
    SM2
}