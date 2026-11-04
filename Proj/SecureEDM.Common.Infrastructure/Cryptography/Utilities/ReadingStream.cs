using System.Buffers;

namespace SecureEDM.Common.Infrastructure.Cryptography.Utilities;

public delegate void ReadingStreamDelegate(byte[] buffer, long totalBytesRead, int bytesRead);

public static class ReadingStream
{
    public static async Task ReadInBuffer(Stream stream, ReadingStreamDelegate action, bool leaveInnerOpen = false, CancellationToken token = default)
    {
        const int bufferSize = 4096;
        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        int totalBytesRead = 0;
        try
        {
            while (true)
            {
                if (token.CanBeCanceled) break;
                var bytesRead = await stream.ReadAtLeastAsync(buffer, 1, false, token);
                if (bytesRead == 0) break;
                action(buffer, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            if (!leaveInnerOpen) await stream.DisposeAsync();
        }
    }
}