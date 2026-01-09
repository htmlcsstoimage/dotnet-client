using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

/// <inheritdoc />
public partial class HtmlCssToImageClient : IHtmlCssToImageClient
{
    private readonly HttpClient _client;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _apiKey;

    private readonly string _apiId;


    /// <summary>
    /// A client for interacting with the HtmlCssToImage service, providing functionality for creating images using HTML and CSS input, managing templates, and generating rendered image URLs.
    /// </summary>
    public HtmlCssToImageClient(HttpClient client, HtmlCssToImageOptions options)
    {
        _client = client;
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HCTIDotNet", LibraryInfo.Version));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.AuthHeader());
        _apiKey = options.ApiKey;
        _apiId = options.ApiId;
    }

    private const string HOST = "https://hcti.io";
    private const string CREATE_PATH = $"{HOST}/v1/image";
    private const string CREATE_AND_RENDER_PATH = $"{CREATE_PATH}/create-and-render";
    private const string CREATE_URL = $"{CREATE_PATH}?includeId=true";
    private const string CREATE_BATCH_URL = $"{CREATE_PATH}/batch";

    private const string TEMPLATE_VERSION_QUERY_PARAM = "template_version";
}