using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HtmlCssToImage.Models;

/// <summary>
/// Represents configuration options for accessing the HtmlCssToImage API, including the necessary API credentials.
/// </summary>
public class HtmlCssToImageOptions
{
    /// <summary>
    /// Your API ID, required for authentication and found on the HtmlCssToImage dashboard.
    /// </summary>
    [Required]
    public required string ApiId { get; set; }

    /// <summary>
    /// Your API key, required for authentication and found on the HtmlCssToImage dashboard.
    /// </summary>
    [Required]
    public required string ApiKey { get; set; }

    /// <summary>
    /// Defines the configuration options required to interact with the HtmlCssToImage API.
    /// This includes API identifiers and keys necessary for authentication.
    /// </summary>
    public HtmlCssToImageOptions() { }

    /// <summary>
    /// Represents the configuration options required to interact with the HtmlCssToImage API.
    /// This includes the API ID and API key, which are mandatory for authentication and usage.
    /// </summary>
    [SetsRequiredMembers]
    public HtmlCssToImageOptions(string apiId, string apiKey)
    {
        ApiId = apiId;
        ApiKey = apiKey;
    }
};