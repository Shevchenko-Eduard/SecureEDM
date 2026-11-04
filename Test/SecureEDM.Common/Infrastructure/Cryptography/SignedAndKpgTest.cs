using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Cryptography.Enums;
using static SecureEDM.Common.Infrastructure.Cryptography.AuxiliaryClass;

namespace SecureEDM.Common.Infrastructure.Cryptography;

public class SignedAndKpgTest
{
    private int SizeTestStream => GetMb(0.01);
    private Stream TestStream
    {
        get
        {
            GetTestStream(SizeTestStream, out var stream);
            return stream;
        }
    }
    [Fact]
    public async Task WhichSignatureWorksWithWhichKpg()
    {
        EnumKpgAlgorithm[] kpgAlgorithms = MapperEnumKpgAlgorithm.Mapper.GetIds();
        EnumSignedAlgorithm[] signedAlgorithms = MapperEnumSignedAlgorithm.Mapper.GetIds();
        Kpg kpg = new();
        Signed signed = new();
        using FileStream fs = new("WhichSignatureWorksWithWhichKpg.log", FileMode.Create, FileAccess.Write);
        using StreamWriter writer = new(fs);
        foreach (var ka in kpgAlgorithms)
        {
            try
            {
                int lengthKey = 256;
                if (ka == EnumKpgAlgorithm.DSA) lengthKey = 1024;
                if (ka == EnumKpgAlgorithm.ML_KEM) lengthKey = 1024;
                var k = await kpg.Generate(ka, lengthKey);
                foreach (var sa in signedAlgorithms)
                {
                    try
                    {
                        await signed.Signing(sa, TestStream, [.. k.PrivateKey]);
                        writer.WriteLine($"[РАБОТАЕТ] {ka} успешно подписал через {sa}");
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine($"[НЕ СОВМЕСТИМЫ] {ka} с {sa} не работает: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                writer.WriteLine($"[КЛЮЧ НЕ РАБОТАЕТ] {ka}: {ex.Message}");
                throw;
            }
        }
    }
}