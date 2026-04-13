using System.Diagnostics;
using System.Net.Http.Headers;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;

namespace HtmlCssToImage;

/// <inheritdoc />
public partial class HtmlCssToImageClient : IHtmlCssToImageClient
{
    private readonly HttpClient _client;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _apiKey;

    private readonly string _apiId;

    private readonly string _apiHost;

    /// <summary>
    /// A client for interacting with the HtmlCssToImage service, providing functionality for creating images using HTML and CSS input, managing templates, and generating rendered image URLs.
    /// </summary>
    public HtmlCssToImageClient(HttpClient client, HtmlCssToImageOptions options): this(client, options, Environment.GetEnvironmentVariable("HCTI_API_BASE_URL") ?? DEFAULT_HOST)
    {
    }

    internal HtmlCssToImageClient(HttpClient client, HtmlCssToImageOptions options, string apiHost)
    {
        _client = client;
        _apiHost = apiHost.TrimEnd('/');
        _client.BaseAddress = new Uri(_apiHost);
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HCTIDotNet", LibraryInfo.Version));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.AuthHeader());
        _apiKey = options.ApiKey;
        _apiId = options.ApiId;
    }

    private const string DEFAULT_HOST = "https://hcti.io";
    private const string CREATE_PATH = "/v1/image";
    private const string CREATE_AND_RENDER_PATH = $"{CREATE_PATH}/create-and-render";
    private const string CREATE_URL = $"{CREATE_PATH}?includeId=true";
    private const string CREATE_BATCH_URL = $"{CREATE_PATH}/batch";

    private const string TEMPLATE_VERSION_QUERY_PARAM = "template_version";
}