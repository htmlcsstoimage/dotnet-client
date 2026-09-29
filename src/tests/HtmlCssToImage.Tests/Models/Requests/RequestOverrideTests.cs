using System.Text.Json;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using Xunit;

namespace HtmlCssToImage.Tests.Models.Requests;

public class RequestOverrideTests
{
    private static readonly RequestOverride[] Rules =
    [
        new()
        {
            Action = RequestOverrideAction.Block,
            Url = "*://example.com/*.js",
            ResourceTypes = [RequestOverrideResourceType.Script, RequestOverrideResourceType.ImageSet, RequestOverrideResourceType.EventSource]
        }
    ];

    [Fact]
    public void ImageRequestsSerializeWireValues()
    {
        var html = new CreateHtmlCssImageRequest { Html = "<p>Hello</p>", RequestOverrides = Rules };
        var url = new CreateUrlImageRequest { Url = "https://example.com", RequestOverrides = Rules };

        AssertRules(JsonSerializer.Serialize(html, JsonContext.Default.CreateHtmlCssImageRequest));
        AssertRules(JsonSerializer.Serialize(url, JsonContext.Default.CreateUrlImageRequest));
    }

    [Fact]
    public void TemplateAndBatchRequestsSerializeRules()
    {
        var template = new CreateTemplateRequest { Html = "<p>Hello</p>", RequestOverrides = Rules };
        var batch = new CreateImageBatchRequest<CreateUrlImageRequest>
        {
            DefaultOptions = new CreateUrlImageRequest { Url = "https://example.com", RequestOverrides = Rules }
        };

        AssertRules(JsonSerializer.Serialize(template, JsonContext.Default.CreateTemplateRequest));
        AssertRules(JsonSerializer.Serialize(batch, JsonContext.Default.CreateImageBatchRequestCreateUrlImageRequest));
    }

    [Fact]
    public void RuleRoundTripsAndSignedUrlOmitsIt()
    {
        var json = JsonSerializer.Serialize(new CreateUrlImageRequest { Url = "https://example.com", RequestOverrides = Rules }, JsonContext.Default.CreateUrlImageRequest);
        var parsed = JsonSerializer.Deserialize(json, JsonContext.Default.CreateUrlImageRequest);
        Assert.Equal(RequestOverrideAction.Block, parsed!.RequestOverrides![0].Action);
        Assert.Equal(Rules[0].ResourceTypes, parsed.RequestOverrides[0].ResourceTypes);

        using var httpClient = new HttpClient();
        var client = new HtmlCssToImageClient(httpClient, new HtmlCssToImageOptions("test_id", "test_key"));
        var signedUrl = client.CreateAndRenderUrl(parsed);
        Assert.DoesNotContain("request_overrides", signedUrl);
    }

    [Fact]
    public void GeneratedEnumStringsKeepExistingWireValues()
    {
        Assert.Equal("png", RenderImageFormat.PNG.ToStringFancy());
        Assert.Equal("dark", ColorSchemeType.dark.ToStringFancy());
        Assert.Equal("print", MediaType.print.ToStringFancy());
        Assert.Equal("%", PdfUnit.PERCENTAGE.ToStringFancy());
        Assert.Equal("center", RenderImageCropOrigin.Center.ToStringFancy());
        Assert.Equal("image_set", RequestOverrideResourceType.ImageSet.ToStringFancy());
    }

    private static void AssertRules(string json)
    {
        using var document = JsonDocument.Parse(json);
        var rule = document.RootElement.TryGetProperty("request_overrides", out var direct)
            ? direct[0]
            : document.RootElement.GetProperty("default_options").GetProperty("request_overrides")[0];

        Assert.Equal("block", rule.GetProperty("action").GetString());
        Assert.Equal("*://example.com/*.js", rule.GetProperty("url").GetString());
        Assert.Equal(new[] { "script", "image_set", "event_source" },
            rule.GetProperty("resource_types").EnumerateArray().Select(value => value.GetString()!).ToArray());
    }
}
