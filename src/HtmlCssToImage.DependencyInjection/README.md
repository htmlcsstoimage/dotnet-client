# [HTML/CSS to Image](https://htmlcsstoimage.com/)
## .NET / C# Client - Dependency Injection

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180-white.png">
  <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
  <img alt="HCTI Logo" src="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
</picture>

This package provides dependency injection support for the HtmlCssToImage client.

[![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.dependencyinjection)](https://www.nuget.org/packages/HtmlCssToImage.DependencyInjection)

## Getting Started

### Installation

Add the package to your project:

```bash
dotnet add package HtmlCssToImage.DependencyInjection
```

### Registering the `IHtmlCssToImageClient` in your application's startup 
#### (usually `Program.cs` or `Startup.cs`)

You can provide your API Id and Key directly or using standard dotnet Configuration.

##### Direct Configuration
If you want to provide your API Id and Key directly, you can do so by calling `AddHtmlCssToImage` in your `Program.cs` or `Startup.cs` - this may be useful for testing purposes locally, but it is not recommended to use plain text credentials in your source code in production.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHtmlCssToImage("api_id", "api_key");
    }
}
```

#### Action-based Configuration
To provide your API Id and Key using an action, you can call `AddHtmlCssToImage` in your `Program.cs` or `Startup.cs` and pass in an action that will be invoked to retrieve your API Id and Key.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHtmlCssToImage(options =>
        {
            options.ApiId = Environment.GetEnvironmentVariable("HCTI_API_ID")!;
            options.ApiKey = Environment.GetEnvironmentVariable("HCTI_API_KEY")!;
        });
    }
}
```

#### Standard dotnet Configuration
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHtmlCssToImage(builder.Configuration.GetSection("HtmlCssToImage"));
    } 
```

In this case, your configuration should look like this:
```json
{
  "HtmlCssToImage":{
    "ApiId": "api_id",
    "ApiKey": "api_key"
  }
}
```

See [HtmlCssToImageOptions.cs](https://github.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/HtmlCssToImageOptions.cs) for the Configuration object 

### HTTP Options
Because the HtmlCssToImage client is added to the DI container as a typed HTTP Client (see [Microsoft Docs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients)) it means you can extend its default behavior, such as adding retry policies or logging. 

All the `.AddHtmlCssToImage()` methods return an `IHttpClientBuilder` which allows you to configure the underlying `HttpClient` instance.

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities.

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).