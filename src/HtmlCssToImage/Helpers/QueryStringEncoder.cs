using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace HtmlCssToImage.Helpers;

internal static class QueryStringEncoder
{
    private static readonly SearchValues<char> UrlSafeChars = SearchValues.Create(
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz-.~");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetMaxChars<T>()
        where T : INumber<T>
    {
        if (typeof(T) == typeof(int)) return 11;
        if (typeof(T) == typeof(uint)) return 10;
        if (typeof(T) == typeof(long)) return 20;
        if (typeof(T) == typeof(ulong)) return 20;
        if (typeof(T) == typeof(float)) return 15;
        if (typeof(T) == typeof(double)) return 25;
        if (typeof(T) == typeof(decimal)) return 31;

        return 32; // fallback
    }

    internal static int EncodeCore(
        scoped ReadOnlySpan<char> input,
        ref ArrayOrSpan<char> destination,
        int position)
    {
        Span<byte> utf8 = stackalloc byte[4];
        var output = destination.Span;

        while (!input.IsEmpty)
        {
            var safeLength = input.IndexOfAnyExcept(UrlSafeChars);
            if (safeLength == -1)
            {
                if (position + input.Length > output.Length)
                {
                    destination.EnsureCapacity(position, input.Length);
                    output = destination.Span;
                }

                input.CopyTo(output[position..]);
                return position + input.Length;
            }

            if (safeLength > 0)
            {
                if (position + safeLength > output.Length)
                {
                    destination.EnsureCapacity(position, safeLength);
                    output = destination.Span;
                }

                input[..safeLength].CopyTo(output[position..]);
                position += safeLength;
                input = input[safeLength..];
            }

            var currentChar = input[0];
            if (char.IsAscii(currentChar))
            {
                if (position + 3 > output.Length)
                {
                    destination.EnsureCapacity(position, 3);
                    output = destination.Span;
                }

                output[position++] = '%';
                output[position++] = GetHexValue(currentChar >> 4);
                output[position++] = GetHexValue(currentChar & 0xF);
                input = input[1..];
                continue;
            }

            var runeStatus = Rune.DecodeFromUtf16(input, out var rune, out var charsConsumed);
            if (runeStatus != OperationStatus.Done)
            {
                input = input[1..];
                continue;
            }

            input = input[charsConsumed..];

            var utf8Length = rune.EncodeToUtf8(utf8);
            var required = utf8Length * 3;
            if (position + required > output.Length)
            {
                destination.EnsureCapacity(position, required);
                output = destination.Span;
            }

            for (var utf8Index = 0; utf8Index < utf8Length; utf8Index++)
            {
                var currentByte = utf8[utf8Index];
                output[position++] = '%';
                output[position++] = GetHexValue(currentByte >> 4);
                output[position++] = GetHexValue(currentByte & 0xF);
            }
        }

        return position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char GetHexValue(int i) => (char)(i < 10 ? i + '0' : i - 10 + 'A');

}
