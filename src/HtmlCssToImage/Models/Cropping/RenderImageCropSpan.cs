namespace HtmlCssToImage.Models;

/// <summary>
/// Defines a valid span along one axis of a crop.
/// </summary>
/// <remarks>
/// A span can extend from a position to the far edge, lie between two positions, use a size after a
/// position, or position a size at the start, center, or end of an axis.
/// </remarks>
public sealed class RenderImageCropSpan
{
    private RenderImageCropSpan(
        RenderImageCropPosition? start,
        RenderImageCropPosition? end,
        RenderImageCropSize? size,
        RenderImageCropOrigin? origin)
    {
        Start = start;
        End = end;
        Size = size;
        Origin = origin;
    }

    /// <summary>
    /// Gets the starting position, or <see langword="null"/> for a size positioned by <see cref="Origin"/>.
    /// </summary>
    public RenderImageCropPosition? Start { get; }

    /// <summary>
    /// Gets the ending position for a bounded span.
    /// </summary>
    public RenderImageCropPosition? End { get; }

    /// <summary>
    /// Gets the span size when the span is size-based.
    /// </summary>
    public RenderImageCropSize? Size { get; }

    /// <summary>
    /// Gets the origin used to position a size-only span, or <see langword="null"/> when the span
    /// has an explicit starting position.
    /// </summary>
    public RenderImageCropOrigin? Origin { get; }

    /// <summary>
    /// Creates a span from a position through the far edge of the axis.
    /// </summary>
    /// <param name="start">The starting position.</param>
    /// <returns>The configured crop span.</returns>
    public static RenderImageCropSpan From(RenderImageCropPosition start)
    {
        start.Validate(nameof(start));
        return new RenderImageCropSpan(start, null, null, null);
    }

    /// <summary>
    /// Creates a span between two positions.
    /// </summary>
    /// <param name="start">The first position.</param>
    /// <param name="end">The second position.</param>
    /// <returns>The configured crop span.</returns>
    public static RenderImageCropSpan Between(RenderImageCropPosition start, RenderImageCropPosition end)
    {
        start.Validate(nameof(start));
        end.Validate(nameof(end));

        if (start.Unit == end.Unit && end.Value <= start.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end.Value, "End must be greater than start when both positions use the same unit.");
        }

        return new RenderImageCropSpan(start, end, null, null);
    }

    /// <summary>
    /// Creates a sized span beginning at a position.
    /// </summary>
    /// <param name="start">The starting position.</param>
    /// <param name="size">The span size.</param>
    /// <returns>The configured crop span.</returns>
    public static RenderImageCropSpan SizedFrom(RenderImageCropPosition start, RenderImageCropSize size)
    {
        start.Validate(nameof(start));
        size.Validate(nameof(size));
        return new RenderImageCropSpan(start, null, size, null);
    }

    /// <summary>
    /// Creates a sized span positioned relative to an axis origin.
    /// </summary>
    /// <param name="size">The span size.</param>
    /// <param name="origin">The position of the span within the axis.</param>
    /// <returns>The configured crop span.</returns>
    public static RenderImageCropSpan Sized(
        RenderImageCropSize size,
        RenderImageCropOrigin origin = RenderImageCropOrigin.Start)
    {
        size.Validate(nameof(size));
        origin.Validate(nameof(origin));
        return new RenderImageCropSpan(null, null, size, origin);
    }
}
