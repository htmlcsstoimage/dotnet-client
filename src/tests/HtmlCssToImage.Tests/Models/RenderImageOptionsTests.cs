using System.Globalization;
using HtmlCssToImage.Models;

namespace HtmlCssToImage.Tests.Models;

public class RenderImageOptionsTests
{
    private const string BaseUrl = "https://hcti.io";
    private const string ImageId = "image-id";

    [Fact]
    public void ToUrl_RectangleBetweenPositions_AppendsCoordinateKeys()
    {
        var crop = RenderImageCrop.Rectangle(
            horizontal: RenderImageCropSpan.Between(
                RenderImageCropPosition.Percent(33),
                RenderImageCropPosition.Percent(66)));

        var result = ToUrl(crop);

        Assert.Equal(
            "https://hcti.io/v1/image/image-id?x_1=33%25&x_2=66%25",
            result);
    }

    [Fact]
    public void ToUrl_RectangleComposesIndependentAxisSpans()
    {
        var crop = RenderImageCrop.Rectangle(
            horizontal: RenderImageCropSpan.SizedFrom(
                RenderImageCropPosition.Pixels(12),
                RenderImageCropSize.Percent(25)),
            vertical: RenderImageCropSpan.From(
                RenderImageCropPosition.Pixels(8)));

        var result = ToUrl(crop);

        Assert.Equal(
            "https://hcti.io/v1/image/image-id?x_1=12px&y_1=8px&crop_width=25%25",
            result);
    }

    [Fact]
    public void ToUrl_RectangleSizeOrigins_AppendsBothOrigins()
    {
        var crop = RenderImageCrop.Rectangle(
            horizontal: RenderImageCropSpan.Sized(
                RenderImageCropSize.Pixels(100),
                RenderImageCropOrigin.Center),
            vertical: RenderImageCropSpan.Sized(
                RenderImageCropSize.Percent(50),
                RenderImageCropOrigin.End));

        var result = ToUrl(crop);

        Assert.Equal(
            "https://hcti.io/v1/image/image-id?x_origin=center&y_origin=end&crop_width=100px&crop_height=50%25",
            result);
    }

    [Fact]
    public void ToUrl_AspectRatioFromWidth_AppendsDerivedHeightOrigin()
    {
        var crop = RenderImageCrop.AspectRatioFromWidth(
            new RenderImageAspectRatio(16, 9),
            RenderImageCropSpan.Between(
                RenderImageCropPosition.Percent(10),
                RenderImageCropPosition.Percent(90)),
            heightOrigin: RenderImageCropOrigin.Center);

        var result = ToUrl(crop);

        Assert.Equal(
            "https://hcti.io/v1/image/image-id?aspect_ratio=16_9&y_origin=center&x_1=10%25&x_2=90%25",
            result);
    }

    [Fact]
    public void ToUrl_AspectRatioFromHeight_AppendsConstrainedAndDerivedOrigins()
    {
        var crop = RenderImageCrop.AspectRatioFromHeight(
            new RenderImageAspectRatio(1, 1),
            RenderImageCropSpan.Sized(
                RenderImageCropSize.Percent(80),
                RenderImageCropOrigin.Center),
            widthOrigin: RenderImageCropOrigin.End);

        var result = ToUrl(crop);

        Assert.Equal(
            "https://hcti.io/v1/image/image-id?aspect_ratio=1_1&x_origin=end&y_origin=center&crop_height=80%25",
            result);
    }

    [Fact]
    public void ToUrl_DefaultOrigins_AreOmitted()
    {
        var crop = RenderImageCrop.AspectRatioFromHeight(
            new RenderImageAspectRatio(1, 1),
            RenderImageCropSpan.Sized(RenderImageCropSize.Percent(100)));

        var result = ToUrl(crop);

        Assert.Equal(
            "https://hcti.io/v1/image/image-id?aspect_ratio=1_1&crop_height=100%25",
            result);
    }

    [Fact]
    public void ToUrl_WithTrailingSlashOnBaseUrl_DoesNotDuplicateSeparator()
    {
        var result = RenderImageOptions.ToUrl(
            $"{BaseUrl}/",
            ImageId,
            new RenderImageOptions());

        Assert.Equal("https://hcti.io/v1/image/image-id", result);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(600)]
    public void ToUrl_WithInvalidDpi_Throws(int dpi)
    {
        var options = new RenderImageOptions { Dpi = (ushort)dpi };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RenderImageOptions.ToUrl(BaseUrl, ImageId, options));
    }

    [Theory]
    [InlineData(31)]
    [InlineData(599)]
    public void ToUrl_WithBoundaryDpi_AppendsDpi(int dpi)
    {
        var result = RenderImageOptions.ToUrl(
            BaseUrl,
            ImageId,
            new RenderImageOptions { Dpi = (ushort)dpi });

        Assert.Equal($"https://hcti.io/v1/image/image-id?dpi={dpi}", result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToUrl_WithZeroDimension_Throws(bool height)
    {
        var options = height
            ? new RenderImageOptions { Height = 0 }
            : new RenderImageOptions { Width = 0 };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RenderImageOptions.ToUrl(BaseUrl, ImageId, options));
    }

    [Fact]
    public void Rectangle_WithoutAnySpan_Throws()
    {
        Assert.Throws<ArgumentException>(() => RenderImageCrop.Rectangle());
    }

    [Fact]
    public void AspectRatio_DefaultValue_ThrowsWhenUsed()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RenderImageCrop.AspectRatioFromWidth(
                default,
                RenderImageCropSpan.From(RenderImageCropPosition.Pixels(10))));
    }

    [Theory]
    [InlineData(0u, 1u)]
    [InlineData(1u, 0u)]
    [InlineData(uint.MaxValue, 1u)]
    public void AspectRatio_InvalidComponents_Throw(uint width, uint height)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RenderImageAspectRatio(width, height));
    }

    [Fact]
    public void Between_NonIncreasingPositionsWithSameUnit_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RenderImageCropSpan.Between(
                RenderImageCropPosition.Pixels(20),
                RenderImageCropPosition.Pixels(10)));
    }

    [Fact]
    public void PositionAndSize_TryParseServerFormats()
    {
        Assert.True(RenderImageCropPosition.TryParse(
            "12",
            CultureInfo.InvariantCulture,
            out var position));
        Assert.Equal(12, position.Value);
        Assert.Equal(RenderImageCropUnit.Pixels, position.Unit);

        Assert.True(RenderImageCropSize.TryParse(
            "33%",
            CultureInfo.InvariantCulture,
            out var size));
        Assert.Equal(33u, size.Value);
        Assert.Equal(RenderImageCropUnit.Percent, size.Unit);

        Assert.True(RenderImageCropSize.TryParse(
            "100PX",
            CultureInfo.InvariantCulture,
            out var pixelSize));
        Assert.Equal(100u, pixelSize.Value);
        Assert.Equal(RenderImageCropUnit.Pixels, pixelSize.Unit);
    }

    [Theory]
    [InlineData("-1px")]
    [InlineData("0%")]
    [InlineData("101%")]
    [InlineData("12.5px")]
    [InlineData("NaNpx")]
    public void Position_TryParseRejectsInvalidValues(string input)
    {
        Assert.False(RenderImageCropPosition.TryParse(
            input,
            CultureInfo.InvariantCulture,
            out _));
    }

    [Theory]
    [InlineData("0px")]
    [InlineData("-1px")]
    [InlineData("0%")]
    [InlineData("101%")]
    [InlineData("33.25%")]
    [InlineData("Infinitypx")]
    public void Size_TryParseRejectsInvalidValues(string input)
    {
        Assert.False(RenderImageCropSize.TryParse(
            input,
            CultureInfo.InvariantCulture,
            out _));
    }

    private static string ToUrl(RenderImageCrop crop)
    {
        return RenderImageOptions.ToUrl(
            BaseUrl,
            ImageId,
            new RenderImageOptions { Crop = crop });
    }
}
