using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace HtmlCssToImage.Helpers;

internal static class QueryStringEncoder
{
    private static readonly SearchValues<char> urlSafeBytes = SearchValues.Create(
        "!()*-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz");

    public static void Write<T>(string key, T value, ref int currCharPosition, ref Span<char> destination, ref char[]? rented) where T : INumber<T>
    {

       var max_key_length= GetEncodedLength(key.AsSpan(), out  var key_needs_encoding);

        var value_length_max = GetMaxChars<T>();
        var max_length = max_key_length + value_length_max +1;
        if (currCharPosition > 0)
        {
            // add one for the '&'
            max_length += 1;
        }
        EnsureCapacity(currCharPosition + max_length, ref currCharPosition, ref destination, ref rented);

        if (currCharPosition > 0)
        {
            destination[currCharPosition++] = '&';
        }
        if (!key_needs_encoding)
        {
            key.AsSpan().CopyTo(destination[currCharPosition..]);
            currCharPosition += key.Length;
        }
        else
        {
            currCharPosition += EncodeCore(key, destination[currCharPosition..]);
        }
        destination[currCharPosition++] = '=';
        if (!value.TryFormat(destination[currCharPosition..], out var chars_written, "R", CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException($"Could not format value {value} into query string for {key}");
        }
        currCharPosition += chars_written;


    }

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



    public static void Encode(string key, string value, ref int currCharPosition, ref Span<char> destination, ref char[]? rented)
    {


        var max_key_length = GetEncodedLength(key.AsSpan(), out var key_needs_encoding);
        var max_value_length = GetEncodedLength(value.AsSpan(), out var value_needs_encoding);

        var max_length = max_key_length + max_value_length +1;
        if (currCharPosition > 0)
        {
            //add one for the '&'
            max_length += 1;
        }
        EnsureCapacity(currCharPosition + max_length, ref currCharPosition, ref destination, ref rented);

        if (currCharPosition > 0)
        {
            destination[currCharPosition++] = '&';
        }

        if (!key_needs_encoding)
        {
            key.AsSpan().CopyTo(destination[currCharPosition..]);
            currCharPosition += key.Length;
        }
        else
        {
            currCharPosition += EncodeCore(key, destination[currCharPosition..]);
        }


        destination[currCharPosition++] = '=';

        if (!value_needs_encoding)
        {
            value.AsSpan().CopyTo(destination[currCharPosition..]);
            currCharPosition += value.Length;
        }
        else
        {
            currCharPosition += EncodeCore(value, destination[currCharPosition..]);
        }



    }

    private static int EncodeCore(ReadOnlySpan<char> input, Span<char> destination)
    {
        int written = 0;

        Span<byte> utf8 = stackalloc byte[4];
        int i = 0;
        while (i<input.Length)
        {
            var rune_status = Rune.DecodeFromUtf16(input[i..], out Rune rune, out int charsConsumed);
            if (rune_status != OperationStatus.Done)
            {
                i++; // INVALID!?
                continue;
            }

            if (rune.IsAscii && urlSafeBytes.Contains((char)rune.Value))
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

    private static char GetHexValue(int i) => (char)(i < 10 ? i + '0' : i - 10 + 'A');


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureCapacity(int requiredCapacity, ref int currCharPosition, ref Span<char> destination, ref char[]? rented)
    {
        if (destination.Length < requiredCapacity)
        {
            // Grow by at least 2x to avoid frequent re-allocations
            int newSize = Math.Max(destination.Length * 2, requiredCapacity);
            char[] newRented = ArrayPool<char>.Shared.Rent(newSize);

            destination[..currCharPosition].CopyTo(newRented);

            if (rented != null)
            {
                ArrayPool<char>.Shared.Return(rented);
            }

            rented = newRented;
            destination = rented;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetEncodedLength(ReadOnlySpan<char> span, out bool needsEncoding)
    {
        int firstUnsafeIndex = span.IndexOfAnyExcept(urlSafeBytes);
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
            if (urlSafeBytes.Contains(c))
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