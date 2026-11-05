namespace SecureEDM.Common.Contracts.Interfaces;

public interface IPoolCache : IDisposable
{
    public Dictionary<object, object> Init(Type? type, string backetName);
    public bool TryGet(Type? type, string backetName, out Dictionary<object, object>? cache);
    public Dictionary<object, object> Get(Type? type, string backetName);
    public void Clear(Type? type, string backetName);
}