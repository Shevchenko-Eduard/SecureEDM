using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Interfaces;

namespace SecureEDM.Common.Infrastructure.Cryptography.Enums;

public class MapperEnumKpgAlgorithm : IMapperEnum<EnumKpgAlgorithm>
{
    public static MapperEnumKpgAlgorithm Mapper = new();
    public MapperEnumKpgAlgorithm() : base(
        new(new Dictionary<EnumKpgAlgorithm, string>{
            { EnumKpgAlgorithm.DH, "DH" },
            { EnumKpgAlgorithm.DSA, "DSA" },
            { EnumKpgAlgorithm.EC, "EC" },
            { EnumKpgAlgorithm.ECDH, "ECDH" },
            { EnumKpgAlgorithm.ECDHC, "ECDHC" },
            { EnumKpgAlgorithm.ECDSA, "ECDSA" },
            { EnumKpgAlgorithm.ECMQV, "ECMQV" },
            { EnumKpgAlgorithm.Ed25519, "Ed25519" },
            { EnumKpgAlgorithm.Ed448, "Ed448" },
            { EnumKpgAlgorithm.ELGAMAL, "ELGAMAL" },
            { EnumKpgAlgorithm.GOST3410, "GOST3410" },
            { EnumKpgAlgorithm.ML_DSA_44, "ML-DSA" },
            { EnumKpgAlgorithm.ML_DSA_65, "ML-DSA" },
            { EnumKpgAlgorithm.ML_DSA_87, "ML-DSA" },
            { EnumKpgAlgorithm.ML_KEM, "ML-KEM" },
            { EnumKpgAlgorithm.RSA, "RSA" },
            { EnumKpgAlgorithm.RSASSA_PSS, "RSASSA-PSS" },
            { EnumKpgAlgorithm.SLH_DSA, "SLH-DSA" },
            { EnumKpgAlgorithm.X25519, "X25519" },
            { EnumKpgAlgorithm.X448, "X448" },
        }))
    { }
}