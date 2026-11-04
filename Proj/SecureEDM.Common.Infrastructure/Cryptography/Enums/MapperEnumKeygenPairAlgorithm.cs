using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Interfaces;

namespace SecureEDM.Common.Infrastructure.Cryptography.Enums;

public class MapperEnumKeygenPairAlgorithm : IMapperEnum<EnumKeygenPairAlgorithm>
{
    public static MapperEnumKeygenPairAlgorithm Mapper = new();

    public MapperEnumKeygenPairAlgorithm() : base(
        new(new Dictionary<EnumKeygenPairAlgorithm, string>{
            { EnumKeygenPairAlgorithm.AIMer, "AIMer" },
            { EnumKeygenPairAlgorithm.BIKE, "BIKE" },
            { EnumKeygenPairAlgorithm.BLS, "BLS" },
            { EnumKeygenPairAlgorithm.CMCE, "CMCE" },
            { EnumKeygenPairAlgorithm.CramerShoup, "CramerShoup" },
            { EnumKeygenPairAlgorithm.DHBasic, "DHBasic" },
            { EnumKeygenPairAlgorithm.DH, "DH" },
            { EnumKeygenPairAlgorithm.Dilithium, "Dilithium" },
            { EnumKeygenPairAlgorithm.DSA, "DSA" },
            { EnumKeygenPairAlgorithm.DSTU4145, "DSTU4145" },
            { EnumKeygenPairAlgorithm.ECCSI, "ECCSI" },
            { EnumKeygenPairAlgorithm.EC, "EC" },
            { EnumKeygenPairAlgorithm.Ed25519, "Ed25519" },
            { EnumKeygenPairAlgorithm.Ed448, "Ed448" },
            { EnumKeygenPairAlgorithm.ElGamal, "ElGamal" },
            { EnumKeygenPairAlgorithm.Faest, "Faest" },
            { EnumKeygenPairAlgorithm.Falcon, "Falcon" },
            { EnumKeygenPairAlgorithm.FrodoKEM, "FrodoKEM" },
            { EnumKeygenPairAlgorithm.Frodo, "Frodo" },
            { EnumKeygenPairAlgorithm.GOST3410, "GOST3410" },
            { EnumKeygenPairAlgorithm.HAETAE, "HAETAE" },
            { EnumKeygenPairAlgorithm.Hawk, "Hawk" },
            { EnumKeygenPairAlgorithm.HQC, "HQC" },
            { EnumKeygenPairAlgorithm.HSS, "HSS" },
            { EnumKeygenPairAlgorithm.LMS, "LMS" },
            { EnumKeygenPairAlgorithm.Mayo, "Mayo" },
            { EnumKeygenPairAlgorithm.MLDSA, "MLDSA" },
            { EnumKeygenPairAlgorithm.MLKEM, "MLKEM" },
            { EnumKeygenPairAlgorithm.MQOM, "MQOM" },
            { EnumKeygenPairAlgorithm.NaccacheStern, "NaccacheStern" },
            { EnumKeygenPairAlgorithm.NH, "NH" },
            { EnumKeygenPairAlgorithm.NTRU, "NTRU" },
            { EnumKeygenPairAlgorithm.NTRULPRime, "NTRULPRime" },
            { EnumKeygenPairAlgorithm.NTRUPlus, "NTRUPlus" },
            { EnumKeygenPairAlgorithm.Picnic, "Picnic" },
            { EnumKeygenPairAlgorithm.QRUOV, "QRUOV" },
            { EnumKeygenPairAlgorithm.Rainbow, "Rainbow" },
            { EnumKeygenPairAlgorithm.RSA, "RSA" },
            { EnumKeygenPairAlgorithm.SABER, "SABER" },
            { EnumKeygenPairAlgorithm.SDitH, "SDitH" },
            { EnumKeygenPairAlgorithm.SLHDSA, "SLHDSA" },
            { EnumKeygenPairAlgorithm.SM2, "SM2" },
            { EnumKeygenPairAlgorithm.Snova, "Snova" },
            { EnumKeygenPairAlgorithm.SNTRUPrime, "SNTRUPrime" },
            { EnumKeygenPairAlgorithm.SPHINCS256, "SPHINCS256" },
            { EnumKeygenPairAlgorithm.SPHINCSPlus, "SPHINCSPlus" },
            { EnumKeygenPairAlgorithm.SQIsign, "SQIsign" },
            { EnumKeygenPairAlgorithm.UOV, "UOV" },
            { EnumKeygenPairAlgorithm.X25519, "X25519" },
            { EnumKeygenPairAlgorithm.X448, "X448" },
            { EnumKeygenPairAlgorithm.XMSS, "XMSS" },
            { EnumKeygenPairAlgorithm.XMSSMT, "XMSSMT" },
            { EnumKeygenPairAlgorithm.XWing, "XWing" },
        }))
    { }
}