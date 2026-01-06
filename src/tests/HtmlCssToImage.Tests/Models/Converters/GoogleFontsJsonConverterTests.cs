using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Tests.Models.Converters;

public class GoogleFontsJsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public GoogleFontsJsonConverterTests()
    {
        _options = new JsonSerializerOptions()
        {
            // Allow '+' and other common URL characters to remain unescaped
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        _options.Converters.Add(new GoogleFontsJsonConverter());
    }
    [Theory]
    [InlineData(new[] { "Open Sans" }, "\"Open+Sans\"")]
    [InlineData(new[] { "Open Sans", "Roboto" }, "\"Open+Sans|Roboto\"")]
    [InlineData(new[] { "  Open Sans  " }, "\"Open+Sans\"")] // Trimming
    [InlineData(new[] { "Open Sans", "Open Sans" }, "\"Open+Sans\"")] // Exact Dupe
    [InlineData(new[] { "Open Sans", "Open+Sans" }, "\"Open+Sans\"")] // Normalized Dupe
    [InlineData(new[] { "Open Sans", "Roboto", "Open Sans" }, "\"Open+Sans|Roboto\"")] // Scattered Dupe
    [InlineData(new[] { "Roboto", "Roboto Condensed" }, "\"Roboto|Roboto+Condensed\"")] // Substring Safety
    public void Write_ShouldNormalizeAndDeduplicate(string[] input, string expectedJson)
    {
        var json = JsonSerializer.Serialize(input, _options);
        Assert.Equal(expectedJson, json);
    }

    [Theory]
    [InlineData("\"Open+Sans\"", new[] { "Open Sans" })]
    [InlineData("\"Open+Sans|Roboto\"", new[] { "Open Sans", "Roboto" })]
    public void Read_ShouldHandlePipesAndPluses(string json, string[] expected)
    {
        var result = JsonSerializer.Deserialize<string[]?>(json, _options);
        Assert.Equal(expected, result);
    }
}