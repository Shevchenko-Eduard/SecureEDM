using System.Text.Json;
using System.Text.Json.Serialization;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Common.Domain.Exceptions;

namespace SecureEDM.Common.Infrastructure.Serializers;

public class SerializerJson<T> : ISerializer<T> 
    where T : class

{
    public static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowTrailingCommas = true,
    };
    public T Deserialize(string value)
    {
        return JsonSerializer.Deserialize<T>(value, options) ?? throw new ExceptionNullableValue();
    }

    public async Task<T> DeserializeAsync(Stream value, CancellationToken token = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(value, options) ?? throw new ExceptionNullableValue();
    }

    public string Serialize(T value)
    {
        return JsonSerializer.Serialize(value, options);
    }

    public async Task<Stream> SerializeAsync(T value, CancellationToken token = default)
    {
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, value, options);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}