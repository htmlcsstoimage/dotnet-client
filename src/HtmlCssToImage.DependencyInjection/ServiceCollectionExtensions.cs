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
    private const string MissingCredentialsMessage = "HtmlCssToImage API credentials must be provided.";

    private static OptionsBuilder<HtmlCssToImageOptions> AddValidatedHtmlCssToImageOptions(this IServiceCollection services)
    {
        return services.AddOptions<HtmlCssToImageOptions>()
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiId) && !string.IsNullOrWhiteSpace(options.ApiKey), MissingCredentialsMessage)
            .ValidateOnStart();
    }

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
            .AddValidatedHtmlCssToImageOptions()
            .Configure(configure);

        return services.ConfigureHtmlCssToImage();
    }

    /// <summary>
    /// Registers the HtmlCssToImage services with the dependency injection system.
    /// This overload allows configuration using services that are already registered in the container.
    /// </summary>
    /// <param name="services">The service collection to which the HtmlCssToImage services will be added.</param>
    /// <param name="configure">An action that can resolve dependencies from the service provider and apply them to the <see cref="HtmlCssToImageOptions"/>.</param>
    /// <returns>
    /// An <see cref="IHttpClientBuilder"/> that can be used to further configure the HTTP client for the HtmlCssToImage service.
    /// </returns>
    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, Action<IServiceProvider, HtmlCssToImageOptions> configure)
    {
        services.AddValidatedHtmlCssToImageOptions();
        services.AddSingleton<IConfigureOptions<HtmlCssToImageOptions>>(sp =>
            new ConfigureNamedOptions<HtmlCssToImageOptions>(string.Empty, options => configure(sp, options)));

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
        services.AddValidatedHtmlCssToImageOptions()
            .Bind(configuration);

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
        services.AddValidatedHtmlCssToImageOptions()
            .BindConfiguration(configSectionPath);

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
        services.AddValidatedHtmlCssToImageOptions()
            .Configure(options =>
            {
                options.ApiId = apiId;
                options.ApiKey = apiKey;
            });

        return services.ConfigureHtmlCssToImage();
    }
}
