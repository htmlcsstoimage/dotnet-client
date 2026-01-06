using System.Text.Json;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Converters;
using Xunit;

namespace HtmlCssToImage.Tests;

public class PdfMarginsConverterTests
{
    private readonly JsonSerializerOptions _options;

    public PdfMarginsConverterTests()
    {
        _options = JsonContext.Default.Options;
    }

    [Fact]
    public void Write_ShouldSerializeToFourElementArray()
    {
        PdfMargins? margins = new PdfMargins(
            new PdfValueWithUnits(10, PdfUnit.PIXELS),
            new PdfValueWithUnits(20, PdfUnit.PIXELS),
            new PdfValueWithUnits(10, PdfUnit.PIXELS),
            new PdfValueWithUnits(20, PdfUnit.PIXELS)
        );

        var json = JsonSerializer.Serialize(margins, _options);

        Assert.Equal("[\"10px\",\"20px\",\"10px\",\"20px\"]", json);
    }

    [Fact]
    public void Read_ShouldParseFourElementArray()
    {
        var json = "[\"5mm\",\"10mm\",\"5mm\",\"10mm\"]";

        var result = JsonSerializer.Deserialize<PdfMargins?>(json, _options);

        Assert.NotNull(result);
        Assert.Equal(5m, result.Value.Top.Value);
        Assert.Equal(PdfUnit.MILLIMETERS, result.Value.Top.Unit);
        Assert.Equal(10m, result.Value.Right.Value);
    }

    [Fact]
    public void Read_ShouldReturnNullForNullToken()
    {
        var result = JsonSerializer.Deserialize<PdfMargins?>("null", _options);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[\"10px\",\"10px\"]")]
    [InlineData("[\"10px\",\"10px\",\"10px\",\"10px\",\"10px\"]")]
    public void Read_ShouldHandleInvalidArrayLengths(string json)
    {
        var result = JsonSerializer.Deserialize<PdfMargins?>(json, _options);

        Assert.Null(result);
    }
}