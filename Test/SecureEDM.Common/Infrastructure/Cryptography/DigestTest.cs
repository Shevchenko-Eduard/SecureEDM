using SecureEDM.Common.Domain.Enums;
using SecureEDM.Common.Infrastructure.Cryptography;
using SecureEDM.Common.Infrastructure.Cryptography.Enums;
using static SecureEDM.Common.Infrastructure.Cryptography.AuxiliaryClass;

namespace SecureEDM.Common.Infrastructure.Cryptography;

public class DigestTest
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
    private EnumDigestAlgorithm[] Algorithms => MapperEnumDigestAlgorithm.Mapper.GetIds();
    private readonly Digest digest = new();
    [Fact]
    public async Task HashedWork()
    {
        var stream = TestStream;
        foreach (var a in Algorithms)
        {
            await digest.Hashing(a, stream, true);
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
    [Fact]
    public async Task HashesAreRepeated()
    {
        var stream = TestStream;
        foreach (var a in Algorithms)
        {
            var res1 = await digest.Hashing(a, stream, true);
            stream.Seek(0, SeekOrigin.Begin);
            var res2 = await digest.Hashing(a, stream, true);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.Equal(res1, res2);
        }
    }
    [Fact]
    public async Task HashesAreNotRepeated()
    {
        var stream1 = TestStream;
        var stream2 = TestStream;
        foreach (var a in Algorithms)
        {
            var res1 = await digest.Hashing(a, stream1, true);
            stream1.Seek(0, SeekOrigin.Begin);
            var res2 = await digest.Hashing(a, stream2, true);
            stream2.Seek(0, SeekOrigin.Begin);
            Assert.NotEqual(res1, res2);
        }
    }
}