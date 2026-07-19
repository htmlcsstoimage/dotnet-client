using System.Globalization;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models;

/// <summary>
/// Defines a positive width-to-height aspect ratio.
/// </summary>
public readonly record struct RenderImageAspectRatio
{
    /// <summary>
    /// Creates an aspect ratio.
    /// </summary>
    /// <param name="width">The positive width component.</param>
    /// <param name="height">The positive height component.</param>
    public RenderImageAspectRatio(uint width, uint height)
    {
        if (width is 0 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, $"Value must be between 1 and {int.MaxValue}.");
        }

        if (height is 0 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, $"Value must be between 1 and {int.MaxValue}.");
        }

        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the width component.
    /// </summary>
    public uint Width { get; }

    /// <summary>
    /// Gets the height component.
    /// </summary>
    public uint Height { get; }

    internal void Validate(string parameterName)
    {
        if (Width is 0 or > int.MaxValue || Height is 0 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Aspect-ratio components must be positive 32-bit integers.");
        }
    }

    internal void AppendToQueryString(
        ref UrlStringBuilder chars,
        ReadOnlySpan<char> key)
    {
        Span<char> value = stackalloc char[21];
        Width.TryFormat(value, out var widthChars, default, CultureInfo.InvariantCulture);
        value[widthChars] = '_';
        Height.TryFormat(value[(widthChars + 1)..], out var heightChars, default, CultureInfo.InvariantCulture);
        chars.EncodeSafeKeyValue(key, value[..(widthChars + heightChars + 1)]);
    }
}
