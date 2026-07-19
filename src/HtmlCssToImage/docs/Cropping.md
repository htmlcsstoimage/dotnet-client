# Cropping with the .NET client

Cropping is configured through `RenderImageOptions.Crop`. The crop is applied first; `Width` and
`Height` then resize the cropped result.

```csharp
using System.Text.Json.Nodes;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;

var options = new RenderImageOptions
{
    Crop = RenderImageCrop.Rectangle(
        horizontal: RenderImageCropSpan.Sized(
            RenderImageCropSize.Percent(80),
            RenderImageCropOrigin.Center))
};
```

## The crop model

A rectangular crop contains a horizontal span, a vertical span, or both:

- The horizontal span selects the left-to-right range.
- The vertical span selects the top-to-bottom range.
- Omitting one span leaves that entire axis uncropped.

Each span can use one of four forms:

| Form | Meaning |
| --- | --- |
| `From(position)` | Start at a position and continue to the far edge. |
| `Between(start, end)` | Crop between two positions. |
| `SizedFrom(position, size)` | Start at an exact position and crop a fixed size. |
| `Sized(size, origin)` | Position a fixed size at the start, center, or end of the axis. |

Positions and sizes support integer pixels or whole percentages:

```csharp
RenderImageCropPosition.Pixels(120);
RenderImageCropPosition.Percent(25);

RenderImageCropSize.Pixels(800);
RenderImageCropSize.Percent(60);
```

Pixel positions may be zero. All sizes must be greater than zero. Percentage positions and sizes
must be from 1 through 100. To begin at the first pixel, use `Pixels(0)` or a size with
`RenderImageCropOrigin.Start`.

## Crop from a position to the edge

Use `From` when only the starting coordinate matters. This example removes the first 200 pixels
from the left and keeps everything to the right:

```csharp
var crop = RenderImageCrop.Rectangle(
    horizontal: RenderImageCropSpan.From(
        RenderImageCropPosition.Pixels(200)));
```

The same operation can be applied vertically. This example removes the top 10 percent:

```csharp
var crop = RenderImageCrop.Rectangle(
    vertical: RenderImageCropSpan.From(
        RenderImageCropPosition.Percent(10)));
```

## Crop between two positions

Use `Between` to provide both boundaries. This keeps the middle third of the image horizontally
and leaves the full height:

```csharp
var crop = RenderImageCrop.Rectangle(
    horizontal: RenderImageCropSpan.Between(
        RenderImageCropPosition.Percent(33),
        RenderImageCropPosition.Percent(66)));
```

Horizontal and vertical spans are independent, so a bounded rectangle can use different units on
each axis:

```csharp
var crop = RenderImageCrop.Rectangle(
    horizontal: RenderImageCropSpan.Between(
        RenderImageCropPosition.Pixels(100),
        RenderImageCropPosition.Pixels(900)),
    vertical: RenderImageCropSpan.Between(
        RenderImageCropPosition.Percent(10),
        RenderImageCropPosition.Percent(90)));
```

Positions within the same span may also mix pixels and percentages. When both positions use the
same unit, the end must be greater than the start.

## Crop a size from an exact position

Use `SizedFrom` when the starting position and crop size are known. The units do not need to match.
This example starts 120 pixels from the left and keeps 50 percent of the source width:

```csharp
var crop = RenderImageCrop.Rectangle(
    horizontal: RenderImageCropSpan.SizedFrom(
        RenderImageCropPosition.Pixels(120),
        RenderImageCropSize.Percent(50)));
```

Both axes can use this form:

```csharp
var crop = RenderImageCrop.Rectangle(
    horizontal: RenderImageCropSpan.SizedFrom(
        RenderImageCropPosition.Pixels(120),
        RenderImageCropSize.Percent(50)),
    vertical: RenderImageCropSpan.SizedFrom(
        RenderImageCropPosition.Pixels(80),
        RenderImageCropSize.Pixels(600)));
```

## Position a crop by its origin

Use `Sized` when the size is known but an exact starting coordinate is not needed:

```csharp
var crop = RenderImageCrop.Rectangle(
    horizontal: RenderImageCropSpan.Sized(
        RenderImageCropSize.Percent(80),
        RenderImageCropOrigin.Center),
    vertical: RenderImageCropSpan.Sized(
        RenderImageCropSize.Pixels(600),
        RenderImageCropOrigin.End));
```

Origins have the following meaning:

| Origin | Horizontal axis | Vertical axis |
| --- | --- | --- |
| `Start` | Left | Top |
| `Center` | Center | Center |
| `End` | Right | Bottom |

`Start` is the default, so these are equivalent:

```csharp
RenderImageCropSpan.Sized(RenderImageCropSize.Pixels(500));

RenderImageCropSpan.Sized(
    RenderImageCropSize.Pixels(500),
    RenderImageCropOrigin.Start);
```

## Enforce an aspect ratio

An aspect-ratio crop defines one axis and calculates the other. Use
`AspectRatioFromWidth` when the horizontal span is known:

```csharp
var crop = RenderImageCrop.AspectRatioFromWidth(
    new RenderImageAspectRatio(16, 9),
    RenderImageCropSpan.Sized(
        RenderImageCropSize.Percent(80),
        RenderImageCropOrigin.Center),
    heightOrigin: RenderImageCropOrigin.Center);
```

In this example, the horizontal span determines the crop width. The API calculates the height
needed for a 16:9 result and centers that calculated height vertically.

Use `AspectRatioFromHeight` when the vertical span is known:

```csharp
var crop = RenderImageCrop.AspectRatioFromHeight(
    new RenderImageAspectRatio(1, 1),
    RenderImageCropSpan.Sized(
        RenderImageCropSize.Percent(80),
        RenderImageCropOrigin.Center),
    widthOrigin: RenderImageCropOrigin.End);
```

Here, the vertical span determines the height. The API calculates the width needed for a square
result and aligns that calculated width to the right.

The origin on the span positions the axis you supplied. `heightOrigin` or `widthOrigin` positions
the other, calculated axis. Both default to `Start`.

## Use the crop in a render URL

The same `RenderImageOptions` can be used for existing images, create-and-render URLs, and template
URLs:

```csharp
var options = new RenderImageOptions
{
    Format = RenderImageFormat.WEBP,
    Width = 1200,
    Crop = RenderImageCrop.Rectangle(
        horizontal: RenderImageCropSpan.Sized(
            RenderImageCropSize.Percent(80),
            RenderImageCropOrigin.Center))
};

var existingImageUrl = client.ImageUrl("image-id", options);

var createAndRenderUrl = client.CreateAndRenderUrl(
    new CreateUrlImageRequest { Url = "https://example.com" },
    options);

var templatedImageUrl = client.CreateTemplatedImageUrl(
    "template-id",
    new JsonObject { ["title"] = "Hello" },
    templateVersion: null,
    options);
```

`Width` and `Height` describe the final output dimensions, not the crop boundaries. If only one is
provided, the cropped image's aspect ratio is preserved.

## Template value collisions

Template values and render options share the URL query string. If your template model has keys that
overlap with render options, the client automatically uses a fallback that the API understands.
Use the typed crop API normally; no special handling is needed.

## Validation summary

- `RenderImageCrop.Rectangle` requires at least one axis.
- Pixel positions must be zero or greater.
- Pixel sizes must be greater than zero.
- Percentage positions and sizes must be from 1 through 100.
- Measurements use whole numbers; fractional pixels and percentages are not supported.
- For a `Between` span whose positions use the same unit, the end must be greater than the start.
- Aspect-ratio width and height components must each be positive 32-bit integers.

For the corresponding raw query parameters, see the API's
[cropping parameters](https://docs.htmlcsstoimage.com/getting-started/using-the-api/#cropping-parameters).
