using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure;

public class FileService(IRootPathRepo rootPath) : IFile
{
    private readonly IRootPathRepo _rootPath = rootPath;
    public async ValueTask<bool> FileExistAsync(string relativePath, CancellationToken token = default)
    {
        var fullPath = await GetFullPath(relativePath);
        return File.Exists(fullPath);
    }

    public async Task<Stream> ReadFileAsync(string relativePath, CancellationToken token = default)
    {
        var fullPath = await GetFullPath(relativePath);
        var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return fs;
    }

    public async Task WriteFileAsync(string relativePath, Stream stream, CancellationToken token = default)
    {
        var fullPath = await GetFullPath(relativePath);
        var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fs);
    }

    public async ValueTask<string> GetRelativePath(string fullPath) => Path.GetRelativePath(await _rootPath.Get(), fullPath);
    public async ValueTask<string> GetFullPath(string relativePath) => Path.GetFullPath(relativePath, await _rootPath.Get());

}