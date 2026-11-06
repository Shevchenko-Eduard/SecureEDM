using System.Text.Json;
using System.Text.Json.Serialization;
using SecureEDM.Client.Application.Interfaces;
using SecureEDM.Client.Application.Models;

namespace SecureEDM.Client.Infrastructure.Storage;

/// <summary>JSON-хранилище реестра в каталоге приложения.</summary>
public sealed class JsonRegistryStore : IRegistryStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    private static string RegistryPath => Path.Combine(AppDataDir.AppData, "Registry.json");

    public async ValueTask<EdoPersistedState?> LoadAsync(CancellationToken token = default)
    {
        if (!File.Exists(RegistryPath)) return null;

        try
        {
            await using var stream = File.OpenRead(RegistryPath);
            return await JsonSerializer.DeserializeAsync<EdoPersistedState>(stream, Options, token);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    public async ValueTask SaveAsync(EdoPersistedState state, CancellationToken token = default)
    {
        var tempPath = RegistryPath + ".tmp";
        await using (var stream = File.Create(tempPath))
            await JsonSerializer.SerializeAsync(stream, state, Options, token);

        File.Move(tempPath, RegistryPath, true);
    }
}
