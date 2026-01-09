using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace HtmlCssToImage.Helpers;

internal static class QueryStringEncoder
{
    private static readonly SearchValues<char> urlSafeChars = SearchValues.Create(
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz-.~");

    public static void Write<T>(string key, T value, ref ArrayOrSpan<char> buffer) where T : INumber<T>
    {
        var max_key_length = GetEncodedLength(key.AsSpan(), out var key_needs_encoding);

        var value_length_max = GetMaxChars<T>();

        var required = max_key_length + value_length_max + 1 + (buffer.Position > 0 ? 1 : 0);

        buffer.EnsureCapacity(required);

        if (buffer.Position > 0)
        {
            buffer.Span[buffer.Position++] = '&';
        }


        if (!key_needs_encoding)
        {
            key.AsSpan().CopyTo(buffer.RemainingSpan);
            buffer.Advance(key.Length);
        }
        else
        {
            buffer.Advance(EncodeCore(key, buffer.RemainingSpan));
        }

        buffer.Span[buffer.Position++] = '=';
        if (!value.TryFormat(buffer.RemainingSpan, out var chars_written, "R", CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException($"Could not format value {value} into query string for {key}");
        }
        buffer.Advance(chars_written);
    }

    public static void WriteSafeKey<T>(ReadOnlySpan<char> key, T value, ref ArrayOrSpan<char> buffer) where T : INumber<T>
    {
        var value_length_max = GetMaxChars<T>();

        var required = key.Length + value_length_max + 1 + (buffer.Position > 0 ? 1 : 0);

        buffer.EnsureCapacity(required);

        var span = buffer.Span;
        var pos = buffer.Position;
        if (pos > 0)
        {
            span[pos++] = '&';
        }

        key.CopyTo(span[pos..]);
        pos+=key.Length;

        span[pos++] = '=';
        if (!value.TryFormat(span[pos..], out var chars_written, "R", CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException($"Could not format value {value} into query string for {key}");
        }
        buffer.Position = pos+chars_written;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetMaxChars<T>()
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

    public static void EncodeSafeKey(ReadOnlySpan<char> key, ReadOnlySpan<char> value, ref ArrayOrSpan<char> buffer)
    {
        var max_value_length = GetEncodedLength(value, out var value_needs_encoding);

        var required = key.Length + max_value_length + 1 + (buffer.Position > 0 ? 1 : 0);
        buffer.EnsureCapacity(required);

        var span = buffer.Span;
        var pos = buffer.Position;

        if (pos > 0)
        {
            span[pos++] = '&';
        }
        key.CopyTo(span[pos..]);
        pos+=key.Length;
        span[pos++] = '=';

        if (!value_needs_encoding)
        {
            value.CopyTo(span[pos..]);
            pos+=value.Length;
        }
        else
        {
            pos += EncodeCore(value, span[pos..]);
        }
        buffer.Position = pos;
    }

    public static void EncodeSafeKeyValue(ReadOnlySpan<char> key, ReadOnlySpan<char> value, ref ArrayOrSpan<char> buffer)
    {

        var required = key.Length + value.Length + 1 + (buffer.Position > 0 ? 1 : 0);
        buffer.EnsureCapacity(required);
        var span = buffer.Span;
        var pos = buffer.Position;
        if (pos > 0)
        {
            span[pos++] = '&';
        }

        key.CopyTo(span[pos..]);
        pos+=key.Length;
        span[pos++] = '=';

        value.CopyTo(span[pos..]);
        buffer.Position = pos+value.Length;
    }

    public static void Encode(ReadOnlySpan<char> key, ReadOnlySpan<char> value, ref ArrayOrSpan<char> buffer)
    {
        var max_key_length = GetEncodedLength(key, out var key_needs_encoding);
        var max_value_length = GetEncodedLength(value, out var value_needs_encoding);

        var required = max_key_length + max_value_length + 1 + (buffer.Position > 0 ? 1 : 0);

        buffer.EnsureCapacity(required);
        var span = buffer.Span;
        var pos = buffer.Position;
        if (pos > 0)
        {
            span[pos++] = '&';
        }

        if (!key_needs_encoding)
        {
            key.CopyTo(span[pos..]);
            pos += key.Length;
        }
        else
        {
            pos += EncodeCore(key, span[pos..]);
        }


        span[pos++] = '=';

        if (!value_needs_encoding)
        {
            value.CopyTo(span[pos..]);
            pos += value.Length;
        }
        else
        {
            pos += EncodeCore(value, span[pos..]);
        }

        buffer.Position = pos;
    }

    private static int EncodeCore(ReadOnlySpan<char> input, Span<char> destination)
    {
        int written = 0;

        Span<byte> utf8 = stackalloc byte[4];
        int i = 0;
        while (i < input.Length)
        {
            int safeLength = input[i..].IndexOfAnyExcept(urlSafeChars);
            if (safeLength == -1)
            {
                input[i..].CopyTo(destination[written..]);
                written += input.Length - i;
                break;
            }

            if (safeLength > 0)
            {
                input[i..(i + safeLength)].CopyTo(destination[written..]);
                written += safeLength;
                i += safeLength;
            }

            var rune_status = Rune.DecodeFromUtf16(input[i..], out Rune rune, out int charsConsumed);
            if (rune_status != OperationStatus.Done)
            {
                i++; // INVALID!?
                continue;
            }

            if (rune.IsAscii && urlSafeChars.Contains((char)rune.Value))
            {
                destination[written++] = (char)rune.Value;
            }
            else
            {
                var utf8_len = rune.EncodeToUtf8(utf8);
                for (var utf8_b = 0; utf8_b < utf8_len; utf8_b++)
                {
                    var this_b = utf8[utf8_b];
                    destination[written++] = '%';
                    destination[written++] = GetHexValue(this_b >> 4);
                    destination[written++] = GetHexValue(this_b & 0xF);
                }
            }

            i += charsConsumed;
        }

        return written;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char GetHexValue(int i) => (char)(i < 10 ? i + '0' : i - 10 + 'A');


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetEncodedLength(ReadOnlySpan<char> span, out bool needsEncoding)
    {
        int firstUnsafeIndex = span.IndexOfAnyExcept(urlSafeChars);
        if (firstUnsafeIndex == -1)
        {
            needsEncoding = false;
            return span.Length;
        }

        needsEncoding = true;
        int length = firstUnsafeIndex;
        // From this point, we need to calculate precisely or use a tighter heuristic.
        // A tight heuristic for AOT/Perf is:
        // 1. Chars before first unsafe: 1:1
        // 2. Remaining chars: assume worst case for those specific remaining chars.
        // BUT: indexAnyExcept is so fast we can just do a quick count.

        for (int i = firstUnsafeIndex; i < span.Length; i++)
        {
            char c = span[i];
            if (urlSafeChars.Contains(c))
            {
                length++;
            }
            else
            {
                // If it's a surrogate pair, it will take 12 chars (%XX%XX%XX%XX)
                // but it's 2 chars in length. So effectively 6x.
                // If it's a standard char, it takes 3 chars (%XX). Effectively 3x.
                length += char.IsHighSurrogate(c) ? 6 : 3;
                if (char.IsHighSurrogate(c))
                {
                    i++;
                }
            }
        }

        return length;
    }
}