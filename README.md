# [HTML/CSS to Image](https://htmlcsstoimage.com/) Dotnet

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

| Package                              | Description                                              | Links |
|--------------------------------------|----------------------------------------------------------| --- |
| **HtmlCssToImage**                   | Core library for working with the API                    | [README](./src/HtmlCssToImage/README.md) |
| **HtmlCssToImage.DependencyInjection** | Dependency injection helpers for ASP.NET Core            | [README](./src/HtmlCssToImage.DependencyInjection/README.md) |
| **HtmlCssToImage.Blazor**                | Blazor component for generating OG image meta tags       | [README](./src/HtmlCssToImage.Blazor/README.md) |
| **HtmlCssToImage.TagHelpers**            | ASP.NET Core TagHelpers for generating OG image meta tags | [README](./src/HtmlCssToImage.TagHelpers/README.md) |

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities. 

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).