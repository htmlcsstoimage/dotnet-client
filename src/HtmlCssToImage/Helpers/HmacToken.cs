using System.Buffers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace HtmlCssToImage.Helpers;

internal static class HmacToken
{
    internal const int TokenLength = 64;

    internal static string CreateToken(in ReadOnlySpan<char> message, in ReadOnlySpan<char> secret)
    {
        Span<char> token = stackalloc char[TokenLength];
        WriteToken(message, secret, token);
        return token.ToString();
    }

    internal static void WriteToken(in ReadOnlySpan<char> message, in ReadOnlySpan<char> secret, Span<char> destination)
    {
        var maximumMessageByteCount = Encoding.UTF8.GetMaxByteCount(message.Length);
        var maximumSecretByteCount = Encoding.UTF8.GetMaxByteCount(secret.Length);

        using ArrayOrSpan<byte> messageBuffer = maximumMessageByteCount <= 256
            ? new(stackalloc byte[256])
            : new(maximumMessageByteCount);
        using ArrayOrSpan<byte> secretBuffer = maximumSecretByteCount <= 256
            ? new(stackalloc byte[256])
            : new(maximumSecretByteCount);

        Span<byte> hash = stackalloc byte[32];
        var messageBytesWritten = Encoding.UTF8.GetBytes(message, messageBuffer.Span);
        var secretBytesWritten = Encoding.UTF8.GetBytes(secret, secretBuffer.Span);

        HMACSHA256.HashData(
            secretBuffer.Span[..secretBytesWritten],
            messageBuffer.Span[..messageBytesWritten],
            hash);

        if (!Convert.TryToHexStringLower(hash, destination, out var charsWritten)
            || charsWritten != TokenLength)
        {
            throw new InvalidOperationException("Could not format the HMAC token.");
        }
    }
}
