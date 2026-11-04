using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace SecureEDM.Common.Infrastructure.Cryptography;

public static class AuxiliaryClass
{
    public static void GetTestStream(string text, out MemoryStream stream)
    {
        stream = new();
        using StreamWriter sw = new(stream, leaveOpen: true);
        sw.Write(text);
        sw.Flush();
        stream.Seek(0, SeekOrigin.Begin);
    }
    public static void GetTestStream(int sizeInBytes, out MemoryStream stream)
    {
        GetTestByteArray(out byte[] bytes, sizeInBytes);
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
    }
    public static void GetTestByteArray(out byte[] bytes, int size)
    {
        bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
    }
    public static MemoryStream CopyStream(Stream stream, bool leaveInnerOpen = false)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
    public static string StreamToString(Stream stream)
    {
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        var sr = new StreamReader(stream, leaveOpen: true);
        var result = sr.ReadToEnd();
        sr.Dispose();
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        return result;
    }
    public static string GetTestString(int size = 1024)
    {
        var bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
        return BitConverter.ToString(bytes);
    }
    public static int GetMb(double mb) => (int)(mb * 1024 * 1024);
    public static int GetKb(double mb) => (int)(mb * 1024);
}