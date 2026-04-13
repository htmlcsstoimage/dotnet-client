using HtmlCssToImage.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HtmlCssToImage.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddHtmlCssToImage_ActionOverload_ConfiguresOptions()
    {
        var services = new ServiceCollection();

        services.AddHtmlCssToImage(options =>
        {
            options.ApiId = "action-api-id";
            options.ApiKey = "action-api-key";
        });

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value;
        var client = serviceProvider.GetRequiredService<IHtmlCssToImageClient>();

        Assert.Equal("action-api-id", options.ApiId);
        Assert.Equal("action-api-key", options.ApiKey);
        Assert.IsType<HtmlCssToImageClient>(client);
    }

    [Fact]
    public void AddHtmlCssToImage_ProviderBasedOverload_ConfiguresOptionsFromRegisteredService()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new TestSecrets
        {
            ApiId = "provider-api-id",
            ApiKey = "provider-api-key"
        });

        services.AddHtmlCssToImage((sp, options) =>
        {
            var secrets = sp.GetRequiredService<TestSecrets>();
            options.ApiId = secrets.ApiId;
            options.ApiKey = secrets.ApiKey;
        });

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value;
        var client = serviceProvider.GetRequiredService<IHtmlCssToImageClient>();

        Assert.Equal("provider-api-id", options.ApiId);
        Assert.Equal("provider-api-key", options.ApiKey);
        Assert.IsType<HtmlCssToImageClient>(client);
    }

    [Fact]
    public void AddHtmlCssToImage_ConfigurationOverload_BindsOptions()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration("configured-api-id", "configured-api-key");

        services.AddHtmlCssToImage(configuration.GetSection("HCTI"));

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value;

        Assert.Equal("configured-api-id", options.ApiId);
        Assert.Equal("configured-api-key", options.ApiKey);
    }

    [Fact]
    public void AddHtmlCssToImage_SectionPathOverload_BindsOptions()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration("path-api-id", "path-api-key");

        services.AddSingleton<IConfiguration>(configuration);
        services.AddHtmlCssToImage("HCTI");

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value;

        Assert.Equal("path-api-id", options.ApiId);
        Assert.Equal("path-api-key", options.ApiKey);
    }

    [Fact]
    public void AddHtmlCssToImage_ApiCredentialsOverload_ConfiguresOptions()
    {
        var services = new ServiceCollection();

        services.AddHtmlCssToImage("direct-api-id", "direct-api-key");

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value;
        var client = serviceProvider.GetRequiredService<IHtmlCssToImageClient>();

        Assert.Equal("direct-api-id", options.ApiId);
        Assert.Equal("direct-api-key", options.ApiKey);
        Assert.IsType<HtmlCssToImageClient>(client);
    }

    [Fact]
    public void AddHtmlCssToImage_InvalidOptions_ThrowsValidationException()
    {
        var services = new ServiceCollection();

        services.AddHtmlCssToImage(options =>
        {
            options.ApiId = string.Empty;
            options.ApiKey = "configured-api-key";
        });

        using var serviceProvider = services.BuildServiceProvider();

        var exception = Assert.Throws<OptionsValidationException>(() =>
            serviceProvider.GetRequiredService<IOptions<HtmlCssToImageOptions>>().Value);

        Assert.Contains("HtmlCssToImage API credentials must be provided.", exception.Failures);
    }

    private static IConfiguration BuildConfiguration(string apiId, string apiKey)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["HCTI:ApiId"] = apiId,
                ["HCTI:ApiKey"] = apiKey
            })
            .Build();
    }

    private sealed class TestSecrets
    {
        public required string ApiId { get; init; }

        public required string ApiKey { get; init; }
    }
}
