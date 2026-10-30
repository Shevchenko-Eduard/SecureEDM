namespace SecureEDM.Common.Domain.Enums;

public enum EnumDigestAlgorithm
{
    // Ascon (NIST SP 800-232)
    AsconCXof128,
    AsconDigest,          // Deprecated: use AsconHash256
    AsconHash256,
    AsconXof,             // Deprecated: use AsconXof128
    AsconXof128,

    // BLAKE
    Blake2b,
    Blake2bp,
    Blake2s,
    Blake2sp,
    Blake2xs,
    Blake3,

    // CSHAKE / SHAKE / SHA-3 / Keccak
    CSHAKE,
    SHAKE,
    SHA3,
    Keccak,

    // GOST / DSTU
    GOST3411_2012_256,
    GOST3411_2012_512,
    GOST3411_2012,        // базовый класс
    GOST3411,             // ГОСТ Р 34.11-94
    DSTU7564,

    // Haraka v2
    Haraka256,
    Haraka512,

    // Легковесная криптография (NIST LWC)
    ISAP,
    PhotonBeetle,
    Romulus,
    Sparkle,
    Xoodyak,

    // KangarooTwelve / MarsupilamiFourteen
    KangarooTwelve,
    MarsupilamiFourteen,

    // MD / SHA / RIPEMD
    MD2,
    MD4,
    MD5,
    SHA1,
    SHA1Interleave,       // RFC 2945, SRP-SHA1
    SHA224,
    SHA256,
    SHA384,
    SHA512,
    SHA512t,
    RIPEMD128,
    RIPEMD160,
    RIPEMD256,
    RIPEMD320,
    Tiger,
    Whirlpool,
    SM3,

    // Skein
    Skein,

    // Специализированные
    // ParallelHash,
    // TupleHash,
    // ShortenedDigest,
    // NullDigest,
    // Prehash,
    // XofUtils,

    // // Обёртки / интерфейсы / базовые классы
    // NonMemoableDigest,
    // EncodableDigest,
    // GeneralDigest,
    // LongDigest
}