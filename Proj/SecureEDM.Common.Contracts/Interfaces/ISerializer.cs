namespace SecureEDM.Common.Contracts.Interfaces;

public interface ISerializer<T> where T : class
{
    string Serialize(T value);
    Task<Stream> SerializeAsync(T value, CancellationToken token = default);
    T Deserialize(string value);
    Task<T> DeserializeAsync(Stream value, CancellationToken token = default);
}