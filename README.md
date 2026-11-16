# [HTML/CSS to Image](https://htmlcsstoimage.com/) 
## .Net / C# Packages

This repo contains a .NET client for the HTML/CSS to Image API, allowing you to generate images and video recordings (MP4, WebM, or GIF) from HTML/CSS content or URLs directly from your .NET applications, as well as helper packages for integrating with ASP.NET Core and Blazor.

## Quick Start


### 1) Add the package to your project:

```bash
dotnet add package HtmlCssToImage
```

### 2) Create a client:

```csharp
var options = new HtmlCssToImageOptions("api-id", "api-key");
var http = new HttpClient();
var client = new HtmlCssToImageClient(http, options);
```

You can also load the credentials from `HCTI_API_ID` and `HCTI_API_KEY`:

```csharp
var options = HtmlCssToImageOptions.FromEnvironmentVariables();
var http = new HttpClient();
var client = new HtmlCssToImageClient(http, options);
```

Pass custom variable names when your application uses different names:

```csharp
var options = HtmlCssToImageOptions.FromEnvironmentVariables(
    "MY_HCTI_API_ID",
    "MY_HCTI_API_KEY");
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

using var html_image = await client.CreateImageAsync(html_request);

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
| **HtmlCssToImage**                     | Core library for working with the API: images, recordings, and templates | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage/README.md)                    | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage)](https://www.nuget.org/packages/HtmlCssToImage)                                         |
| **HtmlCssToImage.DependencyInjection** | Dependency injection helpers for ASP.NET Core                     | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.DependencyInjection/README.md) | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.dependencyinjection)](https://www.nuget.org/packages/HtmlCssToImage.DependencyInjection) |
| **HtmlCssToImage.Blazor**              | Blazor component for generating Open Graph image meta tags        | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.Blazor/README.md)             | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.blazor)](https://www.nuget.org/packages/HtmlCssToImage.Blazor)                           |
| **HtmlCssToImage.TagHelpers**          | ASP.NET Core TagHelpers for generating Open Graph image meta tags | [![README](https://img.shields.io/badge/readme-purple)](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage.TagHelpers/README.md)          | [![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.taghelpers)](https://www.nuget.org/packages/HtmlCssToImage.TagHelpers)                   |

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities. 

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).

## Recordings

Record animated HTML/CSS or a webpage as an MP4, WebM, or GIF with `CreateRecordingAsync`. Pass a `CreateHtmlCssRecordingRequest` for HTML/CSS content:

```csharp
using HtmlCssToImage.Models.Recordings;
using HtmlCssToImage.Models.Requests;

using var html_recording = await client.CreateRecordingAsync(new CreateHtmlCssRecordingRequest
{
    Html = "<h1 class='pulse'>Hello World</h1>",
    Css = "@keyframes pulse { 50% { opacity: 0.3; } } .pulse { animation: pulse 1s infinite; }",
    DurationMs = 3000,
    Output = new() { Format = RecordingFormat.Gif }
});

if(html_recording.Success)
{
    Console.WriteLine(html_recording.Response.RecordingUrl);
}else{
    Console.WriteLine(html_recording.ErrorDetails.Message);
}
```

Or a `CreateUrlRecordingRequest` to record a webpage, with optional timeline actions like scrolling:

```csharp
using var url_recording = await client.CreateRecordingAsync(new CreateUrlRecordingRequest
{
    Url = "https://example.com",
    ViewportWidth = 1280,
    ViewportHeight = 720,
    DurationMs = 10000,
    Actions = [new RecordingSimpleScrollAction()],
    Output = new() { Format = RecordingFormat.Mp4 }
});
```

Use the `RecordingUrl` like an image URL: embed it in a `<video>` tag, link to it, or download it. If the recording isn't finished when the URL is requested, the request waits and then returns it, so you can use the URL right away.

Checking status is optional, for workflows that track progress or failures:

```csharp
using var status = await client.FetchRecordingStatusAsync(url_recording.Response!.Id);
if(status.Success)
{
    Console.WriteLine(status.Response.Status); // Processing, Ready, Failed, ...
}
```

Delete recordings individually or in a batch:

```csharp
using var deleted = await client.DeleteRecordingAsync("recording-id");
using var batch_deleted = await client.DeleteRecordingBatchAsync(["first-id", "second-id"]);
```

See the [recordings guide](https://docs.htmlcsstoimage.com/getting-started/recordings/) and the [package README](https://github.com/htmlcsstoimage/dotnet-client/blob/main/src/HtmlCssToImage/README.md#creating-recordings) for triggers, actions, and output options.

## Templated image batches

Create images from one or more templates with shared defaults and ordered variations:

```csharp
using System.Text.Json.Nodes;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;

using var result = await client.CreateTemplatedImageBatchAsync(new CreateTemplatedImageBatchRequest
{
    DefaultOptions = new() { TemplateId = "t-card", TemplateVersion = 3, Format = RenderImageFormat.WEBP },
    Variations =
    [
        new() { TemplateValues = new JsonObject { ["title"] = "First" } },
        new() { TemplateId = "t-other", TemplateValues = new JsonObject { ["title"] = "Second" } }
    ]
});
```

Omitted fields inherit defaults. Supplying a template ID resets the inherited version; omit its version to use latest. Template value objects merge recursively on the API; arrays, scalars, and explicit null values replace defaults. Results preserve input order and identical images reuse existing assets. Each merged values object must be nonempty and satisfy its template's required variables.

See the [API reference](https://docs.htmlcsstoimage.com/getting-started/using-the-api/#batch-templated-image-creation) for plan limits and examples.
