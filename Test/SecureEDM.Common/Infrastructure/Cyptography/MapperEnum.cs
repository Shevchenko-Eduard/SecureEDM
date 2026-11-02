using SecureEDM.Common.Infrastructure.Cryptography.Enums;

namespace SecureEDM.Common.Infrastructure.Cyptography;

public class MapperEnum
{
    [Fact]
    public void MappedEnumToString()
    {
        var res = MapperEnumDigestAlgorithm.Mapper.TryFromID(Domain.Enums.EnumDigestAlgorithm.SHA_512_224);
        Assert.Equal("SHA-512/224", res);
    }
    [Fact]
    public void MappedStringToEnum()
    {
        var res = MapperEnumDigestAlgorithm.Mapper.TryFromName("SHA-512/224");
        Assert.Equal(Domain.Enums.EnumDigestAlgorithm.SHA_512_224, res);
    }
}