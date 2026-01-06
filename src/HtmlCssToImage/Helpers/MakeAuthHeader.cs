using System.Buffers;
using System.Buffers.Text;
using System.Text;
using HtmlCssToImage.Models;

namespace HtmlCssToImage.Helpers;

internal static class MakeAuthHeader
{
    internal static string AuthHeader(this HtmlCssToImageOptions options)
    {
        var rawByteCount = Encoding.UTF8.GetByteCount(options.ApiId) + 1 + Encoding.UTF8.GetByteCount(options.ApiKey);

        byte[]? rawArrayFromPool = null;
        Span<byte> rawBytes = rawByteCount <= 256
            ? stackalloc byte[256]
            : (rawArrayFromPool = ArrayPool<byte>.Shared.Rent(rawByteCount));
        try
        {
            int written = Encoding.UTF8.GetBytes(options.ApiId, rawBytes);
            rawBytes[written++] = (byte)':';
            written += Encoding.UTF8.GetBytes(options.ApiKey, rawBytes[written..]);

            // Base64 length calculation
            int base64ByteCount = Base64.GetMaxEncodedToUtf8Length(written);

            byte[]? base64ArrayFromPool = null;
            Span<byte> base64Bytes = base64ByteCount <= 512
                ? stackalloc byte[512]
                : (base64ArrayFromPool = ArrayPool<byte>.Shared.Rent(base64ByteCount));

            try
            {
                Base64.EncodeToUtf8(rawBytes[..written], base64Bytes, out _, out int bytesWritten);
                return Encoding.UTF8.GetString(base64Bytes[..bytesWritten]);
            }
            finally
            {
                if (base64ArrayFromPool != null)
                {
                    ArrayPool<byte>.Shared.Return(base64ArrayFromPool);
                }
            }
        }
        finally
        {
            if (rawArrayFromPool != null)
            {
                ArrayPool<byte>.Shared.Return(rawArrayFromPool);
            }
        }


    }
}