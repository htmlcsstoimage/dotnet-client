# [HTML/CSS to Image](https://htmlcsstoimage.com/) 
## .Net / C# Packages

This repo contains a .NET client for the HTML/CSS to Image API, allowing you to generate images from HTML/CSS content directly from your .NET applications, as well as helper packages for integrating with ASP.NET Core and Blazor.

## Quick Start


### 1) Add the package to your project:

```bash
dotnet add package HtmlCssToImage
```

### 2) Create a client:

```csharp
var options = new HtmlCssToImageClientOptions("api-id", "api-key");
var http = new HttpClient();
var client = new HtmlCssToImageClient(http, options);
```

### 3) Generate an image!
```csharp

var html_request = new CreateHtmlCssImageRequest()
{
    Html = "<h1>Hello World</h1>",
    Css = "h1 { color: green; }",
    ViewportWidth = 400,
    ViewportHeight = 200
};

var html_image = await client.CreateImageAsync(html_request);

if(html_image.Success)
{
    Console.WriteLine(html_image.Response.Url);
}else{
    Console.WriteLine(html_image.ErrorDetails.Message);
}

```

## More Details
Check out the package READMEs for more details on integrating:

| Package                                | Description                                                       | Readme                                                                                                                                                                       | Nuget                                                                                                                                                    |                          
|----------------------------------------|-------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| **HtmlCssToImage**                     | Core library for working with the API                             | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage/README.md)                    | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage)](https://www.nuget.org/packages/HtmlCssToImage)                                         |
| **HtmlCssToImage.DependencyInjection** | Dependency injection helpers for ASP.NET Core                     | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.DependencyInjection/README.md) | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.dependencyinjection)](https://www.nuget.org/packages/HtmlCssToImage.DependencyInjection) |
| **HtmlCssToImage.Blazor**              | Blazor component for generating Open Graph image meta tags        | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.Blazor/README.md)             | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.blazor)](https://www.nuget.org/packages/HtmlCssToImage.Blazor)                           |
| **HtmlCssToImage.TagHelpers**          | ASP.NET Core TagHelpers for generating Open Graph image meta tags | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.TagHelpers/README.md)          | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.taghelpers)](https://www.nuget.org/packages/HtmlCssToImage.TagHelpers)                   |

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities. 

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).