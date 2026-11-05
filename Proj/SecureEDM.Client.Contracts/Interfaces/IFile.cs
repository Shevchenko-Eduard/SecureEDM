namespace SecureEDM.Client.Contracts.Interfaces;

public interface IFile
{
    Task<Stream> ReadFileAsync(string relativePath, CancellationToken token = default);
    Task WriteFileAsync(string relativePath, Stream stream, CancellationToken token = default);
    ValueTask<bool> FileExistAsync(string relativePath, CancellationToken token = default);
    ValueTask<string> GetRelativePath(string fullPath);
    ValueTask<string> GetFullPath(string relativePath);
}