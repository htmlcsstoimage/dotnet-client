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
        byte[]? msg_arr = null;
        byte[]? secret_arr = null;
        Span<byte> msg_buffer = max_b_message < 256
            ? stackalloc byte[max_b_message]
            : [];
        Span<byte> secret_buffer = max_b_secret < 256
            ? stackalloc byte[max_b_secret]
            : [];
        if (max_b_message >= 256)
        {
            msg_arr = ArrayPool<byte>.Shared.Rent(max_b_message);
            msg_buffer = msg_arr.AsSpan();
        }

        if (max_b_secret >= 256)
        {
            secret_arr = ArrayPool<byte>.Shared.Rent(max_b_secret);
            secret_buffer = secret_arr.AsSpan();
        }

        Span<byte> hashed_b = stackalloc byte[32];
        var msg_bw = Encoding.UTF8.GetBytes(message, msg_buffer);
        var sec_bw = Encoding.UTF8.GetBytes(secret, secret_buffer);
        HMACSHA256.HashData(secret_buffer.Slice(0, sec_bw), msg_buffer.Slice(0, msg_bw), hashed_b);
        var final = Convert.ToHexStringLower(hashed_b);
        if (msg_arr != null)
        {
            ArrayPool<byte>.Shared.Return(msg_arr);
        }

        if (secret_arr != null)
        {
            ArrayPool<byte>.Shared.Return(secret_arr);
        }

        return final;
    }
}