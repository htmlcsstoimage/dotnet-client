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

        using ArrayOrSpan<byte> rawBytes = rawByteCount <= 256 ? new(stackalloc byte[256]) : new(rawByteCount);


        int written = Encoding.UTF8.GetBytes(options.ApiId, rawBytes.Span);
        rawBytes.Span[written++] = (byte)':';
        written += Encoding.UTF8.GetBytes(options.ApiKey, rawBytes.Span[written..]);

        return Convert.ToBase64String(rawBytes.Span[..written]);
    }
}