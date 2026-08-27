using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HtmlCssToImage.Models;

/// <summary>
/// Represents configuration options for accessing the HtmlCssToImage API, including the necessary API credentials.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
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


    /// <summary>
    /// Creates an instance of <see cref="HtmlCssToImageOptions"/> using API credentials retrieved from environment variables.
    /// </summary>
    /// <param name="apiIdEnvVarName">
    /// The name of the environment variable that contains the API ID. Defaults to "HCTI_API_ID".
    /// </param>
    /// <param name="apiKeyEnvVarName">
    /// The name of the environment variable that contains the API key. Defaults to "HCTI_API_KEY".
    /// </param>
    /// <return>
    /// An instance of <see cref="HtmlCssToImageOptions"/> populated with the API ID and API key retrieved from the specified environment variables.
    /// </return>
    public static HtmlCssToImageOptions FromEnvironmentVariables(string apiIdEnvVarName="HCTI_API_ID", string apiKeyEnvVarName="HCTI_API_KEY")
    {
        return new HtmlCssToImageOptions(Environment.GetEnvironmentVariable(apiIdEnvVarName )!,
            Environment.GetEnvironmentVariable(apiKeyEnvVarName)!);
    }
};