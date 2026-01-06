using HtmlCssToImage.Models;

namespace HtmlCssToImage.Helpers;

internal static class Mappers
{
    internal static string RenderFormatToExtensionWithoutDot(this RenderImageFormat format) => format switch
    {
        RenderImageFormat.PNG => "png",
        RenderImageFormat.JPG => "jpg",
        RenderImageFormat.WEBP => "webp",
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
    internal static string RenderFormatToExtensionWithDot(this RenderImageFormat format) => $".{RenderFormatToExtensionWithoutDot(format)}";

    internal static ReadOnlySpan<char> PdfUnitCssValue(this PdfUnit unit) => unit switch
    {
        PdfUnit.PIXELS => "px",
        PdfUnit.INCHES => "in",
        PdfUnit.PERCENTAGE => "%",
        PdfUnit.CENTIMETERS => "cm",
        PdfUnit.MILLIMETERS => "mm",
        PdfUnit.POINTS => "pt",
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
    };
}