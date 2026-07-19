using System.Text.Json.Nodes;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models;

/// <summary>
/// Defines an immutable rectangular or aspect-ratio crop.
/// </summary>
/// <remarks>
/// Rectangular crops may constrain the horizontal axis, the vertical axis, or both. Aspect-ratio
/// crops constrain exactly one axis and calculate the other from the requested ratio.
/// </remarks>
/// <example>
/// <code>
/// var crop = RenderImageCrop.Rectangle(
///     horizontal: RenderImageCropSpan.Between(
///         RenderImageCropPosition.Percent(33),
///         RenderImageCropPosition.Percent(66)));
///
/// var square = RenderImageCrop.AspectRatioFromHeight(
///     new RenderImageAspectRatio(1, 1),
///     RenderImageCropSpan.Sized(RenderImageCropSize.Percent(100)),
///     widthOrigin: RenderImageCropOrigin.Center);
/// </code>
/// </example>
public sealed class RenderImageCrop
{
    private RenderImageCrop(
        RenderImageAspectRatio? aspectRatio,
        RenderImageCropSpan? horizontal,
        RenderImageCropSpan? vertical,
        RenderImageCropOrigin? otherAxisOrigin)
    {
        AspectRatio = aspectRatio;
        Horizontal = horizontal;
        Vertical = vertical;
        OtherAxisOrigin = otherAxisOrigin;
    }

    /// <summary>
    /// Gets the requested aspect ratio, or <see langword="null"/> for a rectangular crop.
    /// </summary>
    public RenderImageAspectRatio? AspectRatio { get; }

    /// <summary>
    /// Gets the horizontal crop span.
    /// </summary>
    public RenderImageCropSpan? Horizontal { get; }

    /// <summary>
    /// Gets the vertical crop span.
    /// </summary>
    public RenderImageCropSpan? Vertical { get; }

    /// <summary>
    /// Gets the origin of the axis calculated from <see cref="AspectRatio"/>.
    /// </summary>
    public RenderImageCropOrigin? OtherAxisOrigin { get; }

    /// <summary>
    /// Creates a rectangular crop from one or two axis spans.
    /// </summary>
    /// <param name="horizontal">The optional horizontal crop span.</param>
    /// <param name="vertical">The optional vertical crop span.</param>
    /// <returns>The configured crop.</returns>
    /// <exception cref="ArgumentException">Both spans are <see langword="null"/>.</exception>
    public static RenderImageCrop Rectangle(
        RenderImageCropSpan? horizontal = null,
        RenderImageCropSpan? vertical = null)
    {
        if (horizontal == null && vertical == null)
        {
            throw new ArgumentException("At least one crop span must be provided.");
        }

        return new RenderImageCrop(null, horizontal, vertical, null);
    }

    /// <summary>
    /// Creates an aspect-ratio crop whose width is defined by a horizontal span.
    /// </summary>
    /// <param name="aspectRatio">The desired output aspect ratio.</param>
    /// <param name="width">The horizontal span that determines the crop width.</param>
    /// <param name="heightOrigin">The origin used to position the calculated height.</param>
    /// <returns>The configured crop.</returns>
    public static RenderImageCrop AspectRatioFromWidth(
        RenderImageAspectRatio aspectRatio,
        RenderImageCropSpan width,
        RenderImageCropOrigin heightOrigin = RenderImageCropOrigin.Start)
    {
        aspectRatio.Validate(nameof(aspectRatio));
        ArgumentNullException.ThrowIfNull(width);
        heightOrigin.Validate(nameof(heightOrigin));
        return new RenderImageCrop(aspectRatio, width, null, heightOrigin);
    }

    /// <summary>
    /// Creates an aspect-ratio crop whose height is defined by a vertical span.
    /// </summary>
    /// <param name="aspectRatio">The desired output aspect ratio.</param>
    /// <param name="height">The vertical span that determines the crop height.</param>
    /// <param name="widthOrigin">The origin used to position the calculated width.</param>
    /// <returns>The configured crop.</returns>
    public static RenderImageCrop AspectRatioFromHeight(
        RenderImageAspectRatio aspectRatio,
        RenderImageCropSpan height,
        RenderImageCropOrigin widthOrigin = RenderImageCropOrigin.Start)
    {
        aspectRatio.Validate(nameof(aspectRatio));
        ArgumentNullException.ThrowIfNull(height);
        widthOrigin.Validate(nameof(widthOrigin));
        return new RenderImageCrop(aspectRatio, null, height, widthOrigin);
    }

    internal void AppendToQueryString(
        ref UrlStringBuilder chars,
        JsonObject? templateValues = null)
    {
        AspectRatio?.AppendToQueryString(
            ref chars,
            RenderImageOptions.QueryKey(
                templateValues,
                "aspect_ratio",
                "__ro_aspect_ratio"));

        var xOrigin = GetOwnOrigin(Horizontal);
        var yOrigin = GetOwnOrigin(Vertical);

        if (AspectRatio.HasValue && OtherAxisOrigin is not null and not RenderImageCropOrigin.Start)
        {
            if (Horizontal != null)
            {
                yOrigin = OtherAxisOrigin;
            }
            else
            {
                xOrigin = OtherAxisOrigin;
            }
        }

        AppendOriginIfNotNull(
            ref chars,
            xOrigin,
            RenderImageOptions.QueryKey(templateValues, "x_origin", "__ro_x_origin"));
        AppendOriginIfNotNull(
            ref chars,
            yOrigin,
            RenderImageOptions.QueryKey(templateValues, "y_origin", "__ro_y_origin"));
        AppendPositionIfNotNull(
            ref chars,
            Horizontal?.Start,
            RenderImageOptions.QueryKey(templateValues, "x_1", "__ro_x_1"));
        AppendPositionIfNotNull(
            ref chars,
            Horizontal?.End,
            RenderImageOptions.QueryKey(templateValues, "x_2", "__ro_x_2"));
        AppendPositionIfNotNull(
            ref chars,
            Vertical?.Start,
            RenderImageOptions.QueryKey(templateValues, "y_1", "__ro_y_1"));
        AppendPositionIfNotNull(
            ref chars,
            Vertical?.End,
            RenderImageOptions.QueryKey(templateValues, "y_2", "__ro_y_2"));
        AppendSizeWithTemplateFallback(
            ref chars,
            Horizontal?.Size,
            templateValues,
            "crop_width",
            "crop_w",
            "__ro_crop_width",
            "__ro_crop_w");
        AppendSizeWithTemplateFallback(
            ref chars,
            Vertical?.Size,
            templateValues,
            "crop_height",
            "crop_h",
            "__ro_crop_height",
            "__ro_crop_h");
    }

    private static RenderImageCropOrigin? GetOwnOrigin(RenderImageCropSpan? span)
    {
        return span?.Origin is { } origin && origin != RenderImageCropOrigin.Start ? origin : null;
    }

    private static void AppendOriginIfNotNull(
        ref UrlStringBuilder chars,
        RenderImageCropOrigin? origin,
        ReadOnlySpan<char> key)
    {
        if (origin.HasValue)
        {
            origin.Value.AppendToQueryString(key, ref chars);
        }
    }

    private static void AppendPositionIfNotNull(
        ref UrlStringBuilder chars,
        RenderImageCropPosition? position,
        ReadOnlySpan<char> key)
    {
        if (position.HasValue)
        {
            position.Value.AppendToQueryString(ref chars, key);
        }
    }

    private static void AppendSizeWithTemplateFallback(
        ref UrlStringBuilder chars,
        RenderImageCropSize? size,
        JsonObject? templateValues,
        string key,
        string alias,
        string fallbackKey,
        string fallbackAlias)
    {
        if (!size.HasValue)
        {
            return;
        }

        var hasKeyCollision = templateValues?.ContainsKey(key) == true;
        var hasAliasCollision = templateValues?.ContainsKey(alias) == true;

        if (!hasKeyCollision && !hasAliasCollision)
        {
            size.Value.AppendToQueryString(ref chars, key);
            return;
        }

        if (hasKeyCollision)
        {
            size.Value.AppendToQueryString(ref chars, fallbackKey);
        }

        if (hasAliasCollision)
        {
            size.Value.AppendToQueryString(ref chars, fallbackAlias);
        }
    }
}
