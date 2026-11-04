using Org.BouncyCastle.Crypto;
using SecureEDM.Common.Domain.Enums;
using Org.BouncyCastle.Security;
using SecureEDM.Common.Infrastructure.Cryptography.Enums;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Asn1.CryptoPro;

namespace SecureEDM.Common.Infrastructure.Cryptography.Utilities;

public static class KpgAlgorithmUtilities
{
    public static IAsymmetricCipherKeyPairGenerator GetKpg(EnumKpgAlgorithm algorithm)
    {
        return GeneratorUtilities.GetKeyPairGenerator(MapperEnumKpgAlgorithm.Mapper.TryFromID(algorithm));
    }
    public static KeyGenerationParameters GetParameters(this IAsymmetricCipherKeyPairGenerator kpg, EnumKpgAlgorithm algorithm, int lengthKey)
    {
        KeyGenerationParameters? parameters = null;
        switch (kpg)
        {
            case DHKeyPairGenerator:
                var dhParamGen = new DHParametersGenerator();
                dhParamGen.Init(lengthKey, 80, new SecureRandom());
                DHParameters dhParams = dhParamGen.GenerateParameters();
                var dhGenParams = new DHKeyGenerationParameters(new SecureRandom(), dhParams);
                parameters = dhGenParams;
                break;

            case DsaKeyPairGenerator:
                var dsaParamGen = new DsaParametersGenerator();
                dsaParamGen.Init(lengthKey, 80, new SecureRandom());
                DsaParameters dsaParams = dsaParamGen.GenerateParameters();
                var dsaGenParams = new DsaKeyGenerationParameters(new SecureRandom(), dsaParams);
                parameters = dsaGenParams;
                break;

            case ElGamalKeyPairGenerator:
                var elGamalParamGen = new ElGamalParametersGenerator();
                elGamalParamGen.Init(lengthKey, 80, new SecureRandom());
                ElGamalParameters elGamalParams = elGamalParamGen.GenerateParameters();
                var elGamalGenParams = new ElGamalKeyGenerationParameters(new SecureRandom(), elGamalParams);
                parameters = elGamalGenParams;
                break;

            case ECKeyPairGenerator:
                string curveName = lengthKey switch
                {
                    256 => "secp256r1",
                    384 => "secp384r1",
                    521 => "secp521r1",
                    _ => throw new ArgumentException($"No suitable curve was found for length {lengthKey}.")
                };

                X9ECParameters ecP = ECNamedCurveTable.GetByName(curveName)
                    ?? throw new InvalidParameterException($"Unknown curve: {curveName}");
                ECDomainParameters ecSpec = new(ecP);

                var ecGenParms = new ECKeyGenerationParameters(ecSpec, new SecureRandom());
                parameters = ecGenParms;
                break;

            case MLDsaKeyPairGenerator:
                var mlDasParamSet = algorithm switch
                {
                    EnumKpgAlgorithm.ML_DSA_44 => MLDsaParameters.ml_dsa_44_with_sha512,
                    EnumKpgAlgorithm.ML_DSA_65 => MLDsaParameters.ml_dsa_65_with_sha512,
                    EnumKpgAlgorithm.ML_DSA_87 => MLDsaParameters.ml_dsa_87_with_sha512,
                    _ => throw new ArgumentException($"Not soported {algorithm}.")
                };
                var mlDsaGenParms = new MLDsaKeyGenerationParameters(new SecureRandom(), mlDasParamSet);
                parameters = mlDsaGenParms;
                break;

            case MLKemKeyPairGenerator:
                var mlKemParamSet = lengthKey switch
                {
                    512 => MLKemParameters.ml_kem_512,
                    768 => MLKemParameters.ml_kem_768,
                    1024 => MLKemParameters.ml_kem_1024,
                    _ => throw new ArgumentException($"No suitable curve was found for length {lengthKey}.")
                };
                var mlKemGenParms = new MLKemKeyGenerationParameters(new SecureRandom(), mlKemParamSet);
                parameters = mlKemGenParms;
                break;

            case SlhDsaKeyPairGenerator:
                SlhDsaParameters slhDsaParamSet = lengthKey switch
                {
                    128 => SlhDsaParameters.slh_dsa_sha2_128s_with_sha256,
                    192 => SlhDsaParameters.slh_dsa_sha2_192s_with_sha512,
                    256 => SlhDsaParameters.slh_dsa_sha2_256s_with_sha512,
                    _ => throw new ArgumentException($"No suitable curve was found for length {lengthKey}.")
                };
                var slhDsaGenParms = new SlhDsaKeyGenerationParameters(new SecureRandom(), slhDsaParamSet);
                parameters = slhDsaGenParms;
                break;

            default:
                parameters = new KeyGenerationParameters(new SecureRandom(), lengthKey);
                break;
        }

        if (parameters == null) throw new ArgumentNullException($"{kpg} not work.");
        return parameters;
    }
}