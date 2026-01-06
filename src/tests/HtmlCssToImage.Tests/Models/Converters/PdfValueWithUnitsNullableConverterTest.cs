using System.Text.Json;
using System.Text.Json.Serialization;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Tests.Models.Converters;

public class PdfValueWithUnitsNullableConverterTest
{
    private readonly JsonSerializerOptions _jsonOptions;

    public PdfValueWithUnitsNullableConverterTest()
    {
        _jsonOptions = JsonContext.Default.Options;
    }

    [Theory]
    [InlineData("12.34px", 12.34, PdfUnit.PIXELS)]
    [InlineData("50%", 50, PdfUnit.PERCENTAGE)]
    [InlineData("5.2cm", 5.2, PdfUnit.CENTIMETERS)]
    [InlineData("4.56in", 4.56, PdfUnit.INCHES)]
    [InlineData("100mm", 100, PdfUnit.MILLIMETERS)]
    [InlineData("72.5pt", 72.5, PdfUnit.POINTS)]
    public void Read_ValidStringValue_ReturnsExpectedPdfValueWithUnits(string input, decimal expectedValue, PdfUnit expectedUnit)
    {
        var json = $"\"{input}\"";

        var result = JsonSerializer.Deserialize<PdfValueWithUnits?>(json, _jsonOptions);

        Assert.NotNull(result);
        Assert.Equal(expectedValue, result.Value.Value);
        Assert.Equal(expectedUnit, result.Value.Unit);
    }

    [Theory]
    [InlineData("\"\"")]
    public void Read_NullOrEmptyString_ReturnsNull(string input)
    {
        // Act
        var result = JsonSerializer.Deserialize<PdfValueWithUnits?>(input, _jsonOptions);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("\"12.34xyz\"")]
    [InlineData("\"abc%\"")]
    [InlineData("\"123\"")]
    public void Read_InvalidStringFormat_ThrowsJsonException(string input)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PdfValueWithUnits?>(input, _jsonOptions));
    }

    [Theory]
    [InlineData(12.34, PdfUnit.PIXELS, "12.34px")]
    [InlineData(50, PdfUnit.PERCENTAGE, "50%")]
    [InlineData(5.2, PdfUnit.CENTIMETERS, "5.2cm")]
    [InlineData(4.56, PdfUnit.INCHES, "4.56in")]
    [InlineData(100, PdfUnit.MILLIMETERS, "100mm")]
    [InlineData(72.5, PdfUnit.POINTS, "72.5pt")]
    public void Write_ValidPdfValueWithUnits_WritesExpectedJson(decimal value, PdfUnit unit, string expectedJson)
    {
        PdfValueWithUnits? pdfValue = new PdfValueWithUnits(value, unit);

        var result = JsonSerializer.Serialize(pdfValue, _jsonOptions);

        Assert.Equal($"\"{expectedJson}\"", result);
    }

    [Fact]
    public void Write_NullValue_WritesNullJson()
    {
        var result = JsonSerializer.Serialize<PdfValueWithUnits?>(null, _jsonOptions);

        Assert.Equal("null", result);
    }

    [Fact]
    public void Write_LargeDecimalValue_TruncatesToExpectedLength()
    {
        var value = new PdfValueWithUnits(1234567890.123456789m, PdfUnit.PIXELS);

        var result = JsonSerializer.Serialize(value, _jsonOptions);

        Assert.Equal("\"1234567890.1235px\"", result); // Truncated value with unit appended
    }


}