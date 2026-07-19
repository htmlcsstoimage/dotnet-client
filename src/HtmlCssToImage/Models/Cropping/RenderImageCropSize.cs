using System.Diagnostics.CodeAnalysis;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models;

/// <summary>
/// Defines a positive size along a crop axis.
/// </summary>
public readonly record struct RenderImageCropSize : IParsable<RenderImageCropSize>
{
    private RenderImageCropSize(uint value, RenderImageCropUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Gets the numeric value.
    /// </summary>
    public uint Value { get; }

    /// <summary>
    /// Gets the measurement unit.
    /// </summary>
    public RenderImageCropUnit Unit { get; }

    /// <summary>
    /// Creates a pixel size.
    /// </summary>
    /// <param name="value">A pixel value greater than zero.</param>
    /// <returns>The size.</returns>
    public static RenderImageCropSize Pixels(uint value)
    {
        CropHelpers.ValidateSize(value, RenderImageCropUnit.Pixels, nameof(value));
        return new RenderImageCropSize(value, RenderImageCropUnit.Pixels);
    }

    /// <summary>
    /// Creates a percentage size.
    /// </summary>
    /// <param name="value">A percentage greater than zero and no greater than 100.</param>
    /// <returns>The size.</returns>
    public static RenderImageCropSize Percent(uint value)
    {
        CropHelpers.ValidateSize(value, RenderImageCropUnit.Percent, nameof(value));
        return new RenderImageCropSize(value, RenderImageCropUnit.Percent);
    }

    /// <inheritdoc />
    public static RenderImageCropSize Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid crop size.");
    }

    /// <inheritdoc />
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out RenderImageCropSize result)
    {
        result = default;
        if (!CropHelpers.TryParseSize(s, provider, out var value, out var unit) ||
            !CropHelpers.IsValidSize(value, unit))
        {
            return false;
        }

        result = new RenderImageCropSize(value, unit);
        return true;
    }

    internal void Validate(string parameterName)
    {
        CropHelpers.ValidateSize(Value, Unit, parameterName);
    }

    internal void AppendToQueryString(ref QueryStringBuilder chars, ReadOnlySpan<char> key)
    {
        CropHelpers.AppendToQueryString(Value, Unit, key, ref chars);
    }
}
