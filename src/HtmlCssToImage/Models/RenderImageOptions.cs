using System.Text.Json.Nodes;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models;

/// <summary>
/// Defines output format, sizing, and cropping options for rendered images.
/// </summary>
public sealed class RenderImageOptions
{
    /// <summary>
    /// Gets or sets the output image format. PNG is used when no format is specified.
    /// </summary>
    public RenderImageFormat? Format { get; set; }

    /// <summary>
    /// Gets or sets the output DPI metadata value. Valid API values are greater than 30 and less than 600.
    /// </summary>
    public ushort? Dpi { get; set; }

    /// <summary>
    /// Gets or sets the positive output height in pixels. When width is omitted, the source aspect ratio is preserved.
    /// </summary>
    public ushort? Height { get; set; }

    /// <summary>
    /// Gets or sets the positive output width in pixels. When height is omitted, the source aspect ratio is preserved.
    /// </summary>
    public ushort? Width { get; set; }

    /// <summary>
    /// Gets or sets the crop applied before the output dimensions.
    /// </summary>
    public RenderImageCrop? Crop { get; set; }

    internal void AppendToBuilder(
        ref UrlStringBuilder builder,
        bool includeFormat = true,
        JsonObject? templateValues = null)
    {
        Validate();

        if (includeFormat && Format.HasValue)
        {
            builder.AppendLiteral('.');
            builder.AppendLiteral(Format.Value.RenderFormatToExtensionWithoutDot());
        }

        if (Dpi.HasValue)
        {
            builder.WriteSafeKey(
                QueryKey(templateValues, "dpi", "__ro_dpi"),
                Dpi.Value);
        }

        if (Height.HasValue)
        {
            builder.WriteSafeKey(
                QueryKey(templateValues, "height", "__ro_height"),
                Height.Value);
        }

        if (Width.HasValue)
        {
            builder.WriteSafeKey(
                QueryKey(templateValues, "width", "__ro_width"),
                Width.Value);
        }

        Crop?.AppendToQueryString(ref builder, templateValues);
    }

    private void Validate()
    {
        if (Dpi is <= 30 or >= 600)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Dpi),
                Dpi,
                "DPI must be greater than 30 and less than 600.");
        }

        if (Height is 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Height),
                Height,
                "Height must be greater than zero.");
        }

        if (Width is 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Width),
                Width,
                "Width must be greater than zero.");
        }
    }

    internal static string QueryKey(
        JsonObject? templateValues,
        string key,
        string fallbackKey)
    {
        return templateValues?.ContainsKey(key) == true ? fallbackKey : key;
    }

    /// <summary>
    /// Creates the URL for rendering an existing image with the supplied options.
    /// </summary>
    /// <param name="baseUrl">The API base URL.</param>
    /// <param name="imageId">The image identifier.</param>
    /// <param name="options">The rendering options.</param>
    /// <returns>The render URL.</returns>
    /// <exception cref="ArgumentException"><paramref name="baseUrl"/> or <paramref name="imageId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">A DPI or output dimension is outside its valid range.</exception>
    public static string ToUrl(string baseUrl, string imageId, RenderImageOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(imageId);
        ArgumentNullException.ThrowIfNull(options);

        Span<char> initialBuffer = stackalloc char[512];
        UrlStringBuilder builder = new(initialBuffer);
        try
        {
            builder.AppendLiteral(baseUrl.AsSpan().TrimEnd('/'));
            builder.AppendLiteral(HtmlCssToImageClient.CREATE_OR_GET_PATH);
            builder.AppendLiteral('/');
            builder.AppendLiteral(imageId);
            options.AppendToBuilder(ref builder);
            return builder.FullSpan.ToString();
        }
        finally
        {
            builder.Dispose();
        }
    }
}
