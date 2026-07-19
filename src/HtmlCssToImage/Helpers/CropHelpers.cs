using System.Globalization;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models;

internal static class CropHelpers
{
    public static void Validate(this RenderImageCropOrigin origin, string parameterName)
    {
        if (origin is < RenderImageCropOrigin.Start or > RenderImageCropOrigin.End)
        {
            throw new ArgumentOutOfRangeException(parameterName, origin, "Unknown crop origin.");
        }
    }

    public static void AppendToQueryString(
        this RenderImageCropOrigin origin,
        ReadOnlySpan<char> key,
        ref UrlStringBuilder chars)
    {
        ReadOnlySpan<char> value = origin switch
        {
            RenderImageCropOrigin.Start  => "start",
            RenderImageCropOrigin.Center => "center",
            RenderImageCropOrigin.End    => "end",
            _                            => throw new ArgumentOutOfRangeException(nameof(origin), origin, "Unknown crop origin.")
        };

        chars.EncodeSafeKeyValue(key, value);
    }

    public static bool TryParsePosition(
        string? input,
        IFormatProvider? provider,
        out int value,
        out RenderImageCropUnit unit)
    {
        value = default;
        unit = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var span = input.AsSpan().Trim();
        if (!TryReadUnit(ref span, out unit))
        {
            return false;
        }

        return int.TryParse(span, provider, out value);
    }

    public static bool TryParseSize(
        string? input,
        IFormatProvider? provider,
        out uint value,
        out RenderImageCropUnit unit)
    {
        value = default;
        unit = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var span = input.AsSpan().Trim();
        if (!TryReadUnit(ref span, out unit))
        {
            return false;
        }

        return uint.TryParse(span, provider, out value);
    }

    public static bool IsValidPosition(int value, RenderImageCropUnit unit)
    {
        return unit switch
        {
            RenderImageCropUnit.Pixels  => value >= 0,
            RenderImageCropUnit.Percent => value is > 0 and <= 100,
            _                           => false
        };
    }

    public static bool IsValidSize(uint value, RenderImageCropUnit unit)
    {
        return unit switch
        {
            RenderImageCropUnit.Pixels  => value > 0,
            RenderImageCropUnit.Percent => value is > 0 and <= 100,
            _                           => false
        };
    }

    public static void ValidatePosition(int value, RenderImageCropUnit unit, string parameterName)
    {
        if (!IsValidPosition(value, unit))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                unit == RenderImageCropUnit.Percent
                    ? "Percentage positions must be greater than zero and no greater than 100."
                    : "Pixel positions must be non-negative.");
        }
    }

    public static void ValidateSize(uint value, RenderImageCropUnit unit, string parameterName)
    {
        if (!IsValidSize(value, unit))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                unit == RenderImageCropUnit.Percent
                    ? "Percentage sizes must be greater than zero and no greater than 100."
                    : "Pixel sizes must be greater than zero.");
        }
    }

    public static void AppendToQueryString<T>(
        T value,
        RenderImageCropUnit unit,
        ReadOnlySpan<char> key,
        ref UrlStringBuilder chars)
        where T : struct, ISpanFormattable
    {
        Span<char> formatted = stackalloc char[16];
        if (!value.TryFormat(formatted, out var written, default, CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException("Unable to format a crop measurement.");
        }

        if (unit == RenderImageCropUnit.Percent)
        {
            formatted[written++] = '%';
        }
        else
        {
            formatted[written++] = 'p';
            formatted[written++] = 'x';
        }

        chars.EncodeSafeKey(key, formatted[..written]);
    }

    private static bool TryReadUnit(ref ReadOnlySpan<char> span, out RenderImageCropUnit unit)
    {
        if (span.EndsWith("%", StringComparison.Ordinal))
        {
            unit = RenderImageCropUnit.Percent;
            span = span[..^1].TrimEnd();
        }
        else if (span.EndsWith("px", StringComparison.OrdinalIgnoreCase))
        {
            unit = RenderImageCropUnit.Pixels;
            span = span[..^2].TrimEnd();
        }
        else
        {
            unit = RenderImageCropUnit.Pixels;
        }

        return !span.IsEmpty;
    }
}
