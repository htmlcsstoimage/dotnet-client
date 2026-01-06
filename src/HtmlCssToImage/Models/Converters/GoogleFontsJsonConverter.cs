using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Converters;

internal class GoogleFontsJsonConverter : JsonConverter<string[]?>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            return null;
        }


        var google_fonts_str = reader.GetString();
        if (string.IsNullOrWhiteSpace(google_fonts_str))
        {
            return null;
        }

        var span = google_fonts_str.AsSpan();

        if (!span.Contains('|'))
        {
            return [ReplacePlus(span, out var replaced) ? replaced : google_fonts_str];
        }

        var l = new List<string>();
        foreach (var s in span.Split('|'))
        {
            var segment = span[s];
            if (segment.IsWhiteSpace())
            {
                continue;
            }

            if (ReplacePlus(segment, out var replaced))
            {
                l.Add(replaced);

            }
            else
            {
                l.Add(segment.ToString());
            }

        }

        return l.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, string[]? value, JsonSerializerOptions options)
    {
        if (value == null || value.Length == 0)
        {
            writer.WriteNullValue();
            return;
        }

        if (value.Length == 1)
        {
            writer.WriteStringValue(ReplaceSpaceAndTrim(value[0]));
            return;
        }

        var max_len = 0;
        var span = value.AsSpan();
        foreach (var s in span)
        {
            max_len+= s.Length;
        }
        max_len+= span.Length - 1;

        char[]? rented = null;
        // Use stackalloc for small font lists, otherwise rent from pool
        Span<char> buffer = max_len <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(max_len));
        var written = 0;
        for (var i = 0; i < span.Length; i++)
        {
            var start_position = written;
            if (i > 0)
            {
                buffer[written++] = '|';
            }
            var this_font_start = written;
            ReplaceSpaceAndTrim(span[i], ref buffer, ref written);
            if (start_position > 0 && this_font_start > start_position)
            {
                var this_wrote = buffer[this_font_start..written];
                if (IsAlreadyInBuffer(buffer[..this_font_start], this_wrote))
                {
                    written = start_position;
                }
            }

        }
        writer.WriteStringValue(buffer[..written]);

        if (rented != null)
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAlreadyInBuffer(ReadOnlySpan<char> written_buffer, ReadOnlySpan<char> current)
    {
        if (written_buffer.Length == 0)
        {
            return false;
        }

        var start = 0;
        while (start < written_buffer.Length)
        {
            var pipe_index = written_buffer[start..].IndexOf('|');
            var segment_len = (pipe_index == -1) ? written_buffer.Length - start : pipe_index;

            var segment = written_buffer.Slice(start, segment_len);

            // SequenceEqual is SIMD-accelerated and much faster than manual loops
            if (segment.SequenceEqual(current))
            {
                return true;
            }

            if (pipe_index == -1) break;
            start += pipe_index + 1;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ReplaceSpaceAndTrim(string s)
    {
        var span = s.AsSpan().Trim();
        var first_space = span.IndexOf(' ');
        if (first_space == -1 && span.Length==s.Length)
        {
            return s;
        }

        if (first_space == -1)
        {
            return span.ToString();
        }

        return string.Create(span.Length, span, (dest, src) =>
        {
            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];
                dest[i] = (c == ' ') ? '+' : c;
            }

        });

    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReplaceSpaceAndTrim(string s, ref Span<char> buffer, ref int written)
    {
        var span = s.AsSpan().Trim();
        var first_space = span.IndexOf(' ');
        if (first_space == -1)
        {
            span.CopyTo(buffer[written..]);
            written += span.Length;
            return;
        }

        foreach (var c in span)
        {
            buffer[written++] = (c == ' ') ? '+' : c;
        }

    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ReplacePlus(ReadOnlySpan<char> s, [NotNullWhen(true)] out string? result)
    {
        var first_plus = s.IndexOf('+');
        if (first_plus == -1)
        {
            result = null;
            return false;
        }

        result = string.Create(s.Length, s, (dest, src) =>
        {
            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];
                dest[i] = (c == '+') ? ' ' : c;
            }
        });
        return true;
    }
}