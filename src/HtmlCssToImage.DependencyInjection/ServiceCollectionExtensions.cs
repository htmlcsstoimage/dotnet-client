using HtmlCssToImage;
using HtmlCssToImage.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains extension methods for registering HtmlCssToImage services with the
/// dependency injection system using various configuration options.
/// </summary>
public static class ServiceCollectionExtensions
{

    private static IHttpClientBuilder ConfigureHtmlCssToImage(this IServiceCollection services)
    {
        return services.AddHttpClient<IHtmlCssToImageClient, HtmlCssToImageClient>((client, sp) =>
        {
            var options = sp.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value;
            return new HtmlCssToImageClient(client, options);
        });
    }

    /// <summary>
    /// Registers the HtmlCssToImage services with the dependency injection system.
    /// This method allows configuration of the API options for interacting with the HtmlCssToImage service.
    /// </summary>
    /// <param name="services">The service collection to which the HtmlCssToImage services will be added.</param>
    /// <param name="configure">An action to configure the <see cref="HtmlCssToImageOptions"/>.</param>
    /// <returns>
    /// An <see cref="IHttpClientBuilder"/> that can be used to further configure the HTTP client for the HtmlCssToImage service.
    /// </returns>
    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, Action<HtmlCssToImageOptions> configure)
    {
        services
            .AddOptions<HtmlCssToImageOptions>()
            .Configure(configure)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiId) && !string.IsNullOrWhiteSpace(options.ApiKey), "HtmlCssToImage API credentials must be provided.")
            .ValidateOnStart();

        return services.ConfigureHtmlCssToImage();
    }

    /// <summary>
    /// Registers the HtmlCssToImage services with the dependency injection system.
    /// This overload allows configuration using an <see cref="IConfiguration"/> instance.
    /// </summary>
    /// <param name="services">The service collection to which the HtmlCssToImage services will be added.</param>
    /// <param name="configuration">An <see cref="IConfiguration"/> instance containing the settings for <see cref="HtmlCssToImageOptions"/>.</param>
    /// <returns>
    /// An <see cref="IHttpClientBuilder"/> that can be used to further configure the HTTP client for the HtmlCssToImage service.
    /// </returns>
    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<HtmlCssToImageOptions>()
            .Bind(configuration)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiId) && !string.IsNullOrWhiteSpace(options.ApiKey), "HtmlCssToImage API credentials must be provided.")
            .ValidateOnStart();

        return services.ConfigureHtmlCssToImage();
    }

    /// <summary>
    /// Registers the HtmlCssToImage services using a configuration section path.
    /// This is the preferred method for Native AOT and trimming compatibility.
    /// </summary>
    /// <param name="services">The service collection to which the HtmlCssToImage services will be added.</param>
    /// <param name="configSectionPath">The path to the configuration section (e.g., "HtmlCssToImage").</param>
    /// <returns>
    /// An <see cref="IHttpClientBuilder"/> that can be used to further configure the HTTP client for the HtmlCssToImage service.
    /// </returns>
    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, string configSectionPath)
    {
        services.AddOptions<HtmlCssToImageOptions>()
            .BindConfiguration(configSectionPath)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiId) && !string.IsNullOrWhiteSpace(options.ApiKey), "HtmlCssToImage API credentials must be provided.")
            .ValidateOnStart();

        return services.ConfigureHtmlCssToImage();
    }



    /// <summary>
    /// Registers the HtmlCssToImage services with the dependency injection system.
    /// This overload allows configuration of the API credentials directly using the provided API ID and API Key.
    /// </summary>
    /// <param name="services">The service collection to which the HtmlCssToImage services will be added.</param>
    /// <param name="apiId">The API ID required to authenticate with the HtmlCssToImage service.</param>
    /// <param name="apiKey">The API Key required to authenticate with the HtmlCssToImage service.</param>
    /// <returns>
    /// An <see cref="IHttpClientBuilder"/> that can be used to further configure the HTTP client for the HtmlCssToImage service.
    /// </returns>
    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, string apiId, string apiKey)
    {
        services.AddOptions<HtmlCssToImageOptions>()
            .Configure(options =>
            {
                options.ApiId = apiId;
                options.ApiKey = apiKey;
            })
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiId) && !string.IsNullOrWhiteSpace(options.ApiKey), "HtmlCssToImage API credentials must be provided.")
            .ValidateOnStart();
        return services.ConfigureHtmlCssToImage();
    }


}