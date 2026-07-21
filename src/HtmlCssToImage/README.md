# [HTML/CSS to Image](https://htmlcsstoimage.com/) 
## .NET / C# Client

![HCTI Logo](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png)


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

If you're using ASP.NET Core or similar frameworks supporting Microsoft DI, check out the [HtmlCssToImage.DependencyInjection Package Docs](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.DependencyInjection/README.md) for how to inject the client into your application.

## Response Shape

Most client methods that call the HCTI API return an [`ApiResult<T>`](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage/Models/Results/ApiResult.cs). Signed URL helpers return a `string` instead because they do not make an HTTP request.

An `ApiResult<T>` provides:

- `Response`: The typed response body when the request succeeds.
- `ErrorDetails`: The typed API error when the request fails.
- `Success` and `StatusCode`: A quick way to inspect the outcome.
- `HttpResponseMessage`: The raw HTTP response for advanced cases.

The raw `HttpResponseMessage` is included so you can inspect response headers, the originating `RequestMessage`, the HTTP version, reason phrase, and other transport-level details that are not part of the typed response.

`ApiResult<T>` owns that raw response and implements `IDisposable`. Use `using` or `using var` for every result so the underlying response and content are released promptly.

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

using var html_image = await client.CreateImageAsync(html_request);

if(html_image.Success)
{
    Console.WriteLine(html_image.Response.Url);
}else{
    Console.WriteLine(html_image.ErrorDetails.Message);
}
```

## Deleting Images

Delete one or more existing images from HCTI and clear them from the CDN using their image IDs:

```csharp
using var result = await client.DeleteImageAsync("your-image-id");

if (!result.Success)
{
    Console.WriteLine(result.ErrorDetails.Message);
}

using var batchResult = await client.DeleteImageBatchAsync(
    ["first-image-id", "second-image-id"]);
```

### Template Helpers
When creating a templated image, you can use static helper methods `FromObject<T>` on the `CreateTemplatedImageRequest` class to generate a template, providing serialization options for AOT/serialization control. See more below in the [Performance & Native AOT](#performance--native-aot) section.
 
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


Batch creation responds with an [`ApiResult<CreateImageResponse[]>`](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage/Models/Responses/CreateImageResponse.cs). If your request is successful, the `Response` property will be an array in the order of your `variations`.

## Creating Image URLs

You can generate signed URLs for images without actually calling the API by calling `CreateAndRenderUrl` or one of the `CreateTemplatedImageUrl` methods. 

These methods are synchronous because they don't make any network calls or do heavy IO, and have been designed to be very high performance.

Read more about signed URLs in the [create-and-render docs](https://docs.htmlcsstoimage.com/getting-started/create-and-render/).

These URLs are tied to the API Key & API Id you provide when creating the client. If you change them or disable the keys, you'll need to generate new URLs.

The signature covers the complete query string, including output sizing and cropping options. If you change any query parameter, generate a new signed URL.

These methods are handy when you have a lot of content that may never be rendered, and want to render on-demand, as to not waste your image credits.

### Output Sizing and Cropping

Use `RenderImageOptions` with an existing image, a create-and-render request, or a template URL:

```csharp
var renderOptions = new RenderImageOptions
{
    Format = RenderImageFormat.WEBP,
    Dpi = 144,
    Width = 1200,
    Crop = RenderImageCrop.Rectangle(
        horizontal: RenderImageCropSpan.Sized(
            RenderImageCropSize.Percent(80),
            RenderImageCropOrigin.Center))
};

var existingImageUrl = client.ImageUrl("image-id", renderOptions);

var createAndRenderUrl = client.CreateAndRenderUrl(
    new CreateUrlImageRequest { Url = "https://example.com" },
    renderOptions);

var templatedImageUrl = client.CreateTemplatedImageUrl(
    "template-id",
    new JsonObject { ["title"] = "Hello" },
    templateVersion: null,
    renderOptions);
```

Crop positions and sizes use integer pixels or whole percentages. The [.NET cropping guide](docs/Cropping.md) walks through every crop shape, unit, and origin with complete examples. See the API's [cropping parameters](https://docs.htmlcsstoimage.com/getting-started/using-the-api/#cropping-parameters) for the corresponding raw query parameters.

DPI must be greater than 30 and less than 600; width and height must be greater than zero. If your template model has keys that overlap with render options, the client automatically uses a fallback that the API understands.

## Performance & Native AOT

This library is built with performance in mind and is fully compatible with **Native AOT** (Ahead-of-Time) compilation in .NET 9+. 

The client internally uses source-generated JSON serialization, so no extra configuration is required for standard API requests.

### AOT with Templated Images

When using templates, you provide a custom object for `templateValues`. In a Native AOT environment, reflection is restricted, so you must ensure your types are source-generated. Ensure you use the overloads with `JsonTypeInfo<T>` or `JsonSerializerOptions` to provide serialization options. If you're providing `JsonSerializerOptions`, ensure it has the type info in its resolver chain. Check out the [Microsoft JSON docs](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation) for more details.

## Other Packages

### HtmlCssToImage.DependencyInjection
Use [HtmlCssToImage.DependencyInjection](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.DependencyInjection/README.md) to integrate the HtmlCssToImage client into your ASP.NET Core application.

### HtmlCssToImage.Blazor
Use [HtmlCssToImage.Blazor](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.Blazor/README.md) to generate Open Graph image tags using HCTI links in your Blazor applications.

### HtmlCssToImage.TagHelpers
Use [HtmlCssToImage.TagHelpers](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.TagHelpers/README.md) to generate Open Graph image tags in ASP.NET Core Razor Pages and MVC applications.

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities.

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).
