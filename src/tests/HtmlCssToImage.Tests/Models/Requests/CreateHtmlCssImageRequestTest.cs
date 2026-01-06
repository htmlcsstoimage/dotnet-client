using System.Text.Json;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;

namespace HtmlCssToImage.Tests.Models.Requests;

public class CreateHtmlCssImageRequestTest
{

    private const string BasicTestHtmlSerialized = """
                                                   {"html":"\u003Cb\u003ETest\u003C/b\u003E"}
                                                   """;

    private const string SerializedWithGoogleFontsSingle = """
                                                           {"html":"\u003Cb\u003ETest\u003C/b\u003E","google_fonts":"Open\u002BSans"}
                                                           """;
    private static readonly string[] GoogleFontsSingle = new[] {"Open Sans"};



    [Fact]
    public void SerializeCorrectly()
    {
        var req = new CreateHtmlCssImageRequest {Html = "<b>Test</b>"};
        var serialized = JsonSerializer.Serialize(req, JsonContext.Default.CreateHtmlCssImageRequest);

        Assert.Equal(BasicTestHtmlSerialized, serialized);

    }

    [Fact]
    public void DeserializeCorrectly()
    {
        var req = JsonSerializer.Deserialize<CreateHtmlCssImageRequest>(BasicTestHtmlSerialized, JsonContext.Default.CreateHtmlCssImageRequest);

        Assert.Equal("<b>Test</b>", req?.Html);
    }

    [Fact]
    public void SerializeCorrectlyWithGoogleFonts()
    {
        var req = new CreateHtmlCssImageRequest {Html = "<b>Test</b>", GoogleFonts = GoogleFontsSingle};
        var serialized = JsonSerializer.Serialize(req, JsonContext.Default.CreateHtmlCssImageRequest);
        Assert.Equal(SerializedWithGoogleFontsSingle,serialized);
    }
    [Fact]
    public void DeserializeCorrectlyWithGoogleFonts()
    {
        var req = JsonSerializer.Deserialize<CreateHtmlCssImageRequest>(SerializedWithGoogleFontsSingle, JsonContext.Default.CreateHtmlCssImageRequest);
        Assert.Equal("<b>Test</b>", req?.Html);
        Assert.Equal(GoogleFontsSingle, req?.GoogleFonts);
    }

    [Fact]
    public void SerializeCorrectlyWithPdfOptions()
    {

        var expected_serialized = """
                                  {"html":"\u003Cb\u003ETest\u003C/b\u003E","pdf_options":{"page_height":"400px","page_width":"500px"}}
                                  """;

        var req = new CreateHtmlCssImageRequest()
        {
            Html = "<b>Test</b>",
            PDFOptions = new PDFOptions()
            {
                PageHeight = 400,
                PageWidth = 500
            }
        };
        Assert.NotNull(req.PDFOptions.PageHeight);
        Assert.Equal(PdfUnit.PIXELS, req.PDFOptions.PageHeight.Value.Unit);
        var serialized = JsonSerializer.Serialize(req, JsonContext.Default.CreateHtmlCssImageRequest);
        Assert.Equal(expected_serialized,serialized);
    }

    [Fact]
    public void SerializeCorrectlyWithPdfOptionsHeightInches()
    {

        var expected_serialized = """
                                  {"html":"\u003Cb\u003ETest\u003C/b\u003E","pdf_options":{"page_height":"10in"}}
                                  """;

        var req = new CreateHtmlCssImageRequest()
        {
            Html = "<b>Test</b>",
            PDFOptions = new PDFOptions()
            {
                PageHeight = PdfValueWithUnits.Inches(10)
            }
        };
        Assert.NotNull(req.PDFOptions.PageHeight);
        Assert.Equal(PdfUnit.INCHES, req.PDFOptions.PageHeight.Value.Unit);
        var serialized = JsonSerializer.Serialize(req, JsonContext.Default.CreateHtmlCssImageRequest);
        Assert.Equal(expected_serialized,serialized);
    }

    [Fact]
    public void SerializeCorrectlyWithPdfOptionsMargins()
    {

        var expected_serialized = """
                                  {"html":"\u003Cb\u003ETest\u003C/b\u003E","pdf_options":{"margins":["0.5in","0.5in","0.5in","1in"]}}
                                  """;

        var half_inch = PdfValueWithUnits.Inches((decimal)0.5);
        var one_inch = PdfValueWithUnits.Inches(1);
        var req = new CreateHtmlCssImageRequest()
        {
            Html = "<b>Test</b>",
            PDFOptions = new PDFOptions()
            {
                Margins = new PdfMargins(half_inch,half_inch,half_inch,one_inch)
            }
        };

        var serialized = JsonSerializer.Serialize(req, JsonContext.Default.CreateHtmlCssImageRequest);
        Assert.Equal(expected_serialized,serialized);
    }


}