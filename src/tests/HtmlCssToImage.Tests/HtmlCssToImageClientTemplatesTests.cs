using System.Net;
using System.Net.Http.Json;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;
using Moq;
using Moq.Protected;
using Xunit;

namespace HtmlCssToImage.Tests;

public class HtmlCssToImageClientTemplatesTests
{
    private readonly HtmlCssToImageOptions _options;
    private readonly Mock<HttpMessageHandler> _handlerMock;

    public HtmlCssToImageClientTemplatesTests()
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

    [Theory]
    [InlineData(null, 10u, null, "https://hcti.io/v1/template?count=10")]
    [InlineData("tpl_123", 20u, null, "https://hcti.io/v1/template/tpl_123?count=20")]
    [InlineData(null, 50u, 12345L, "https://hcti.io/v1/template?count=50&max_version=12345")]
    [InlineData("tpl_456", 100u, 67890L, "https://hcti.io/v1/template/tpl_456?count=100&max_version=67890")]
    public void GetTemplateListUrl_GeneratesCorrectUrl(string? templateId, uint count, long? nextPageStart, string expectedUrl)
    {
        var url = HtmlCssToImageClient.GetTemplateListUrl(templateId, count, nextPageStart);
        Assert.Equal(expectedUrl, url);
    }

    [Fact]
    public async Task ListTemplates_ReturnsSuccessResult()
    {
        var client = CreateClient();
        var expectedResponse = new PaginatedResponse<Template>
        {
            Data = [new Template { TemplateId = "tpl_1", Name = "Template 1" }],
            Pagination = new PaginatedResponse<Template>.PaginationInfo(123)
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get && m.RequestUri!.ToString().Contains("/v1/template?count=10")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResponse, JsonContext.Default.PaginatedResponseTemplate)
            });

        var result = await client.ListTemplatesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response.Data);
        Assert.Equal("tpl_1", result.Response.Data[0].TemplateId);
        Assert.Equal(123L, result.Response.Pagination.next_page_start);
    }

    [Fact]
    public async Task ListTemplateVersions_ReturnsSuccessResult()
    {
        var client = CreateClient();
        var templateId = "tpl_123";
        var expectedResponse = new PaginatedResponse<Template>
        {
            Data = [new Template { TemplateId = templateId, TemplateVersion = 1 }],
            Pagination = new PaginatedResponse<Template>.PaginationInfo(null)
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get && m.RequestUri!.ToString().Contains($"/v1/template/{templateId}?count=20")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResponse, JsonContext.Default.PaginatedResponseTemplate)
            });

        var result = await client.ListTemplateVersionsAsync(templateId, count: 20, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal(templateId, result.Response.Data[0].TemplateId);
    }

    [Fact]
    public async Task ListTemplatesCore_HandlesErrorResponse()
    {
        var client = CreateClient();
        var errorDetails = new ErrorDetails { Message = "Not Authorized" };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = JsonContent.Create(errorDetails, JsonContext.Default.ErrorDetails)
            });

        var result = await client.ListTemplatesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
        Assert.NotNull(result.ErrorDetails);
        Assert.Equal("Not Authorized", result.ErrorDetails.Message);
    }

    [Theory]
    [InlineData(0, 10u)] // Should default to 10
    [InlineData(101, 100u)] // Should cap at 100
    public async Task ListTemplates_ClampsCountValue(int inputCount, uint expectedUsedCount)
    {
        var client = CreateClient();
        
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.ToString().Contains($"count={expectedUsedCount}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new PaginatedResponse<Template> { Data = [], Pagination = new PaginatedResponse<Template>.PaginationInfo(null) }, JsonContext.Default.PaginatedResponseTemplate)
            });

        await client.ListTemplatesAsync(count: inputCount, cancellationToken: TestContext.Current.CancellationToken);
        
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.ToString().Contains($"count={expectedUsedCount}")),
            ItExpr.IsAny<CancellationToken>());
    }
}
