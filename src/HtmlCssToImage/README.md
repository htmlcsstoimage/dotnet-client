# [HTML/CSS to Image](https://htmlcsstoimage.com/) 
## .NET / C# Client

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180-white.png">
  <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
  <img alt="HCTI Logo" src="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
</picture>

This package provides a .NET client for the HTML/CSS to Image API, allowing you to generate images from HTML/CSS content directly from your .NET applications.

[![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage)](https://www.nuget.org/packages/HtmlCssToImage)

## Getting Started

### Installation

Add the package to your project:

```bash
dotnet add package HtmlCssToImage
```

## Configuration

Creating a new instance of the `HtmlCssToImageClient`:

```csharp
var options = new HtmlCssToImageClientOptions("api-id", "api-key");
var http = new HttpClient();
var client = new HtmlCssToImageClient(http, options);
```

If you're using ASP.NET Core or similar frameworks supporting Microsoft DI, check out the [HtmlCssToImage.DependencyInjection Package Docs](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage.DependencyInjection/README.md) for how to inject the client into your application.


## Creating Images
You can generate images using your HCTI client with strongly typed parameters:

| Type                                                                        | Description                                        |
|-----------------------------------------------------------------------------|----------------------------------------------------|
| [`CreateHtmlCssImageRequest`](Models/Requests/CreateHtmlCssImageRequest.cs) | The request object for creating an image from HTML & CSS content |
| [`CreateUrlImageRequest`](Models/Requests/CreateUrlImageRequest.cs)           | The request object for creating an image from a URL|
| [`CreateTemplatedImageRequest`](Models/Requests/CreateTemplatedImageRequest.cs) | The request object for creating an image from a template |

Here's a basic example of creating an image from HTML & CSS content:
```csharp

var html_request = new CreateHtmlCssImageRequest()
{
    Html = "<h1>Hello World</h1>",
    Css = "h1 { color: red; }",
    ViewportWidth = 200,
    ViewportHeight = 400
};

var html_image = await client.CreateImageAsync(html_request);

if(html_image.Success)
{
    Console.WriteLine(html_image.Response.Url);
}else{
    Console.WriteLine(html_image.ErrorDetails.Message);
}
```

Image creation responds with an [`ApiResult<CreateImageResponse>`](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/Responses/CreateImageResponse.cs). [`ApiResult<T>`](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/Results/ApiResult.cs) is a simple wrapper around Api responses that provides an indicator of `Success` and potentially `ErrorDetails` if the request failed.

### Template Helpers
When creating a templated image, you can use static helper methods `FromObject<T>` on the `CreateTemplatedImageRequest` class to generate a template, providing serialization options for AOT/serialization control. 
 
```csharp 
// Create a template from an object, using default serialization options. This will warn in AOT scenarios
public static CreateTemplatedImageRequest CreateTemplatedImageRequest.FromObject<T>(T templateValuies, string templateId, long? templateVersion = null)
    
// Create a template from an object, providing JsonSerializationOptions for serialization control
public static CreateTemplatedImageRequest FromObject<T>(T templateValues, string templateId, JsonSerializerOptions jsonSerializerOptions, long? templateVersion = null) 

// Create a template from an object, providing JsonTypeInfo<T>
public static CreateTemplatedImageRequest FromObject<T>(T templateValues, string templateId, JsonTypeInfo<T> typeInfo, long? templateVersion = null) 
```

## Creating an Image Batch

Call `CreateImageBatchAsync<T>` on the client to create a batch of images from a collection of either `CreateHtmlCssImageRequest` or `CreateUrlImageRequest` objects. At this time, templates are not supported in batch requests.

You can construct a `CreateImageBatchRequest` object directly or use the overload on `HtmlCssToImageClient` that accepts `defaultOptions` and `variations` as parameters.


Batch creation responds with an [`ApiResult<CreateImageResponse[]>`](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/Responses/CreateImageResponse.cs). [`ApiResult<T>`](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/Results/ApiResult.cs) is a simple wrapper around Api responses that provides an indicator of `Success` and potentially `ErrorDetails` if the request failed.

If your request is successful, the `Response` property will be an array in the order of your `variations`.

## Creating Image URLs

You can generate signed URLs for images without actually calling the API by calling `CreateAndRenderUrl` or one of the `CreateTemplatedImageUrl` methods. 

These methods are synchronous because they don't make any network calls or do heavy IO, and have been designed to be very high performance.

Read more about signed URLs in the [create-and-render docs](https://docs.htmlcsstoimage.com/getting-started/create-and-render/).

These URLs are tied to the API Key & API Id you provide when creating the client. If you change them or disable the keys, you'll need to generate new URLs.

These methods are handy when you have a lot of content that may never be rendered, and want to render on-demand, as to not waste your image credits.


## Other Packages

### HtmlCssToImage.DependencyInjection
Use [HtmlCssToImage.DependencyInjection](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage.DependencyInjection/README.md) to integrate the HtmlCssToImage client into your ASP.NET Core application.

### HtmlCssToImage.Blazor
Use [HtmlCssToImage.Blazor](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage.Blazor/README.md) to generate Open Graph image tags using HCTI links in your Blazor applications.

### HtmlCssToImage.TagHelpers
Use [HtmlCssToImage.TagHelpers](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage.TagHelpers/README.md) to generate Open Graph image tags in ASP.NET Core Razor Pages and MVC applications.

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities.

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).