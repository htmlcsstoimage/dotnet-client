using System.Diagnostics.CodeAnalysis;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models;

/// <summary>
/// Defines a non-negative position along a crop axis.
/// </summary>
public readonly record struct RenderImageCropPosition : IParsable<RenderImageCropPosition>
{
    private RenderImageCropPosition(int value, RenderImageCropUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Gets the numeric value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Gets the measurement unit.
    /// </summary>
    public RenderImageCropUnit Unit { get; }

    /// <summary>
    /// Creates a pixel position.
    /// </summary>
    /// <param name="value">A non-negative pixel value.</param>
    /// <returns>The position.</returns>
    public static RenderImageCropPosition Pixels(int value)
    {
        CropHelpers.ValidatePosition(value, RenderImageCropUnit.Pixels, nameof(value));
        return new RenderImageCropPosition(value, RenderImageCropUnit.Pixels);
    }

    /// <summary>
    /// Creates a percentage position.
    /// </summary>
    /// <param name="value">A percentage greater than zero and no greater than 100.</param>
    /// <returns>The position.</returns>
    public static RenderImageCropPosition Percent(int value)
    {
        CropHelpers.ValidatePosition(value, RenderImageCropUnit.Percent, nameof(value));
        return new RenderImageCropPosition(value, RenderImageCropUnit.Percent);
    }

    /// <inheritdoc />
    public static RenderImageCropPosition Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid crop position.");
    }

    /// <inheritdoc />
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out RenderImageCropPosition result)
    {
        result = default;
        if (!CropHelpers.TryParsePosition(s, provider, out var value, out var unit) ||
            !CropHelpers.IsValidPosition(value, unit))
        {
            return false;
        }

        result = new RenderImageCropPosition(value, unit);
        return true;
    }

    internal void Validate(string parameterName)
    {
        CropHelpers.ValidatePosition(Value, Unit, parameterName);
    }

    internal void AppendToQueryString(ref QueryStringBuilder chars, ReadOnlySpan<char> key)
    {
        CropHelpers.AppendToQueryString(Value, Unit, key, ref chars);
    }
}
