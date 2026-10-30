namespace SecureEDM.Common.Domain.Enums;

public enum EnumPairKeygenAlgorithm
{
    // AIM / BIKE / BLS
    AIMer,
    BIKE,
    BLS,

    // CMCE 
    CMCE,

    // Классические
    CramerShoup,
    DHBasic,
    DH,
    DSA,
    DSTU4145,
    EC,
    Ed25519,
    Ed448,
    ElGamal,
    GOST3410,
    NaccacheStern,
    RSA,
    SM2,
    X25519,
    X448,

    // ECCSI
    ECCSI,

    // Пост-квантовые: решётки / коды / хеш-базированные / прочие
    Dilithium,           // = ML-DSA (старое имя)
    MLDSA_44,           // указан дважды (разные параметризации: 44/65/87)
    MLDSA_65, 
    MLDSA_87,
    Falcon,
    HAETAE,
    Hawk,
    MLKEM_512,          // указан дважды (512/768/1024)
    MLKEM_768, 
    MLKEM_1024,
    NTRU,
    NTRULPRime,
    NTRUPlus,
    SNTRUPrime,
    SABER,
    FrodoKEM,
    Frodo,
    HQC,
    BIKE_,
    CMCE_,
    Picnic,
    Rainbow,
    QRUOV,
    UOV,
    Snova,
    MAYO,
    SQIsign,
    MQOM,
    SDitH,
    Faest,
    AIMer_,
    SLHDSA_128, 
    SLHDSA_192, 
    SLHDSA_256,

    // Хеш-базированные подписи
    HSS,
    LMS,
    SPHINCS256,
    SPHINCSPlus,
    XMSS,
    XMSSMT,

    // Гибридные KEM
    XWing,

    // Специальные
    NH
}