using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Common.Infrastructure;

public class MapperEnumDigestAlgorithm : IMapperEnum<EnumDigestAlgorithm>
{
    static MapperEnumDigestAlgorithm()
    {
        IdToName = new(new Dictionary<EnumDigestAlgorithm, string>{
            { EnumDigestAlgorithm.MD2, "MD2" },
            { EnumDigestAlgorithm.MD4, "MD4" },
            { EnumDigestAlgorithm.MD5, "MD5" },
            { EnumDigestAlgorithm.SHA_1, "SHA-1" },
            { EnumDigestAlgorithm.SHA_224, "SHA-224" },
            { EnumDigestAlgorithm.SHA_256, "SHA-256" },
            { EnumDigestAlgorithm.SHA_384, "SHA-384" },
            { EnumDigestAlgorithm.SHA_512, "SHA-512" },
            { EnumDigestAlgorithm.SHA_512_224, "SHA-512/224" },
            { EnumDigestAlgorithm.SHA_512_256, "SHA-512/256" },
            { EnumDigestAlgorithm.RIPEMD128, "RIPEMD128" },
            { EnumDigestAlgorithm.RIPEMD160, "RIPEMD160" },
            { EnumDigestAlgorithm.RIPEMD256, "RIPEMD256" },
            { EnumDigestAlgorithm.RIPEMD320, "RIPEMD320" },
            { EnumDigestAlgorithm.GOST3411, "GOST3411" },
            { EnumDigestAlgorithm.KECCAK_224, "KECCAK-224" },
            { EnumDigestAlgorithm.KECCAK_256, "KECCAK-256" },
            { EnumDigestAlgorithm.KECCAK_288, "KECCAK-288" },
            { EnumDigestAlgorithm.KECCAK_384, "KECCAK-384" },
            { EnumDigestAlgorithm.KECCAK_512, "KECCAK-512" },
            { EnumDigestAlgorithm.SHA3_224, "SHA3-224" },
            { EnumDigestAlgorithm.SHA3_256, "SHA3-256" },
            { EnumDigestAlgorithm.SHA3_384, "SHA3-384" },
            { EnumDigestAlgorithm.SHA3_512, "SHA3-512" },
            { EnumDigestAlgorithm.SHAKE128_256, "SHAKE128-256" },
            { EnumDigestAlgorithm.SHAKE256_512, "SHAKE256-512" },
            { EnumDigestAlgorithm.SM3, "SM3" },
            { EnumDigestAlgorithm.BLAKE2B_160, "BLAKE2B-160" },
            { EnumDigestAlgorithm.BLAKE2B_256, "BLAKE2B-256" },
            { EnumDigestAlgorithm.BLAKE2B_384, "BLAKE2B-384" },
            { EnumDigestAlgorithm.BLAKE2B_512, "BLAKE2B-512" },
            { EnumDigestAlgorithm.BLAKE2S_128, "BLAKE2S-128" },
            { EnumDigestAlgorithm.BLAKE2S_160, "BLAKE2S-160" },
            { EnumDigestAlgorithm.BLAKE2S_224, "BLAKE2S-224" },
            { EnumDigestAlgorithm.BLAKE2S_256, "BLAKE2S-256" },
            { EnumDigestAlgorithm.BLAKE3_256, "BLAKE3-256" },
            { EnumDigestAlgorithm.GOST3411_2012_256, "GOST3411-2012-256" },
            { EnumDigestAlgorithm.GOST3411_2012_512, "GOST3411-2012-512" },
            { EnumDigestAlgorithm.DSTU7564_256, "DSTU7564-256" },
            { EnumDigestAlgorithm.DSTU7564_384, "DSTU7564-384" },
            { EnumDigestAlgorithm.DSTU7564_512, "DSTU7564-512" },
        });
    }
}