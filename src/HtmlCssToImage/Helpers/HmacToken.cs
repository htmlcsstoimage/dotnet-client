using System.Buffers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace HtmlCssToImage.Helpers;

internal static class HmacToken
{
    internal static string CreateToken(in ReadOnlySpan<char> message, in ReadOnlySpan<char> secret)
    {
        var max_b_message = Encoding.UTF8.GetMaxByteCount(message.Length);
        var max_b_secret = Encoding.UTF8.GetMaxByteCount(secret.Length);

        using ArrayOrSpan<byte> msg_buffer = max_b_message<=256? new(stackalloc byte[256]): new(max_b_message);
        using ArrayOrSpan<byte> secret_buffer = max_b_secret<=256? new(stackalloc byte[256]): new(max_b_secret);


        Span<byte> hashed_b = stackalloc byte[32];
        var msg_bw = Encoding.UTF8.GetBytes(message, msg_buffer.Span);
        var sec_bw = Encoding.UTF8.GetBytes(secret, secret_buffer.Span);
        HMACSHA256.HashData(secret_buffer.Span.Slice(0, sec_bw), msg_buffer.Span.Slice(0, msg_bw), hashed_b);
        var final = Convert.ToHexStringLower(hashed_b);
        return final;
    }
}