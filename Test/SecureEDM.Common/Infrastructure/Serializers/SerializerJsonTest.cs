using SecureEDM.Common.Infrastructure.Cryptography;

namespace SecureEDM.Common.Infrastructure.Serializers;

public class SerializerJsonTest
{
    private record TestObject(
        int Id,
        string Name,
        int[] FriendsId,
        Uri Uri,
        TimeSpan TimeSpan,
        TestEnum TestEnum,
        bool False = false,
        bool True = true,
        string? Null = null);
    private static readonly TestObject testObject = new(
        Id: 69,
        Name: "Bob",
        FriendsId: [.. Enumerable.Range(2, 5)],
        new("http://test.org/"),
        new(1, 2, 3),
        TestEnum.TestEnum1
    );
    [Fact]
    public void SerializeAndDeserialize()
    {
        var s = new SerializerJson<TestObject>();
        var stringObject = s.Serialize(testObject);
        Console.WriteLine(stringObject);
        var resObject = s.Deserialize(stringObject);
        AssertEqual(testObject, resObject);
    }
    [Fact]
    public async Task SerializeAsyncAndDeserialize()
    {
        var s = new SerializerJson<TestObject>();
        var streamObject = await s.SerializeAsync(testObject);
        var stringObject = AuxiliaryClass.StreamToString(streamObject);
        var resObject = s.Deserialize(stringObject);
        AssertEqual(testObject, resObject);
    }
    [Fact]
    public async Task SerializeAndDeserializeAsync()
    {
        var s = new SerializerJson<TestObject>();
        var stringObject = s.Serialize(testObject);
        AuxiliaryClass.GetTestStream(stringObject, out var streamObject);
        var resObject = await s.DeserializeAsync(streamObject);
        AssertEqual(testObject, resObject);
    }
    private static void AssertEqual(TestObject expected, TestObject actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.FriendsId, actual.FriendsId);
        Assert.Equal(expected.False, actual.False);
        Assert.Equal(expected.True, actual.True);
        Assert.Equal(expected.Null, actual.Null);
    }
    private enum TestEnum
    {
        TestEnum1,
        TestEnum2
    }
}