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

    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, Action<HtmlCssToImageOptions> configure)
    {
        services.Configure(configure);
        return services.ConfigureHtmlCssToImage();
    }

    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HtmlCssToImageOptions>(configuration);
        return services.ConfigureHtmlCssToImage();
    }

    public static IHttpClientBuilder AddHtmlCssToImage(this IServiceCollection services, string apiId, string apiKey)
    {
        services.Configure<HtmlCssToImageOptions>(options =>
        {
            options.ApiId = apiId;
            options.ApiKey = apiKey;
        });
        return services.ConfigureHtmlCssToImage();
    }




}