using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using Moq;
using Moq.Protected;

namespace HtmlCssToImage.Tests;

public class HtmlCssToImageClientTests
{
    private readonly HtmlCssToImageOptions _options;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    public HtmlCssToImageClientTests()
    {
        _options = new HtmlCssToImageOptions
        {
            ApiKey = "test_key",
            ApiId = "test_id"
        };
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    }


    private HtmlCssToImageClient CreateClient()
    {
        var httpClient = new HttpClient(_handlerMock.Object);
        return new HtmlCssToImageClient(httpClient, _options);
    }

    public enum QueryStringType
    {
        None,
        Numbers,
        Emoji,
        Long,
        Space
    }

    [Theory]
    [InlineData(QueryStringType.None,null)]
    [InlineData(QueryStringType.None,RenderImageFormat.PNG)]
    [InlineData(QueryStringType.None,RenderImageFormat.JPG)]
    [InlineData(QueryStringType.None,RenderImageFormat.WEBP)]
    [InlineData(QueryStringType.Numbers,null)]
    [InlineData(QueryStringType.Numbers,RenderImageFormat.WEBP)]
    [InlineData(QueryStringType.Emoji,null)]
    [InlineData(QueryStringType.Long,null)]
    [InlineData(QueryStringType.Space,null)]
    public void CreateUrl_GeneratesValidSignedUrl(QueryStringType type, RenderImageFormat? format  )
    {
        var qs = type switch
        {
            QueryStringType.None => "",
            QueryStringType.Numbers => "?abc=123",
            QueryStringType.Emoji => "?abc=👀",
            QueryStringType.Long => $"?abc={new string('a', 600)}",
            QueryStringType.Space => "?abc=a b",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        var client = CreateClient();
        var request = new CreateUrlImageRequest { Url = $"https://google.com{qs}" };
        var result = client.CreateAndRenderUrl(request, format?? RenderImageFormat.PNG);
        var uri = new Uri(result);
        Assert.StartsWith("https://hcti.io/v1/image/create-and-render/test_id/", result);
        Assert.Equal($"?url={Uri.EscapeDataString(request.Url)}", uri.Query);

        var parts = uri.AbsolutePath.Split('/');
        var token_part = parts[^1];
        if (format != null && format != RenderImageFormat.PNG)
        {
            token_part = parts[^2];
            Assert.Equal(parts.Last(), format.ToString()!.ToLower());
        }
        Assert.Equal(HexLowerHmac(_options.ApiKey, uri.Query[1..]), token_part);
    }


    [Fact]
    public void CreateTemplatedImageUrl_WithVersion_IncludesTemplateVersionInQuery()
    {
        var client = CreateClient();
        var values = new JsonObject { ["title"] = "Hello" };

        var url = client.CreateTemplatedImageUrl("tpl_123", values, templateVersion: 5);

        Assert.Contains("template_version=5", url);
        Assert.Contains("title=%22Hello%22", url);
    }

    [Fact]
    public async Task CreateImageAsync_WhenSuccess_ReturnsResponseData()
    {
        var client = CreateClient();
        var request = new CreateHtmlCssImageRequest { Html = "<b>Test</b>" };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { url = "https://hcti.io/v1/image/img_1", id = "img_1" })
            });

        var result = await client.CreateImageAsync(request, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal("img_1", result.Response.Id);
    }

    [Fact]
    public async Task CreateImageBatchAsync_WhenError_PopulatesErrorDetails()
    {
        var client = CreateClient();
        var request = new CreateImageBatchRequest<CreateHtmlCssImageRequest>();

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = JsonContent.Create(new { message = "Invalid request", error = "missing_params" })
            });

        var result = await client.CreateImageBatchAsync(request, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.NotNull(result.ErrorDetails);
        Assert.Equal("Invalid request", result.ErrorDetails.Message);
    }



    private static string HexLowerHmac(string key, string value)
    {
       return Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(value)));
    }
}