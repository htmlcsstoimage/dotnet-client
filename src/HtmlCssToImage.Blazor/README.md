# [HTML/CSS to Image](https://htmlcsstoimage.com/) 
## .NET / C# Client - Blazor Integration

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180-white.png">
  <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
  <img alt="HCTI Logo" src="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
</picture>

This package provides Blazor integration for generating Open Graph meta tags using the HtmlCssToImage client.

[![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.blazor)](https://www.nuget.org/packages/HtmlCssToImage.Blazor)

## Getting Started

### Installation

Add the package to your project:

```bash
dotnet add package HtmlCssToImage.Blazor
```

### Configuration

You'll also need to register the `IHtmlCssToImageClient` in your application's startup (usually `Program.cs` or `Startup.cs`):

```csharp
builder.Services.AddHtmlCssToImage("api_id", "api_key");
```

Refer to the [HtmlCssToImage.DependencyInjection docs](https://github.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage.DependencyInjection/README.md) for more details on configuring the client.


### Setup

In your `App.razor` you need to add a `HCTIOutlet` to your `head` tag to include the generated meta tags on all pages:
```razor
<head>
 <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <base href="/"/>
    
    <ImportMap/>
    <HeadOutlet/>
    <HCTIOutlet AdditionalMetaTypes="@(["twitter:image"])"></HCTIOutlet>
</head>
```

You can specify additional meta types to generate automatically by passing them to the `HCTIOutlet` component in the `AdditionalMetaTypes` parameter. By default, `og:image` will be generated. You can also specify them on an individual basis.

## Blazor Components

There are 2 available components, to inject `meta` tags for your pages.

### `HCTI_Templated_OgMetaTag`

Use the `HCTI_Templated_OgMetaTag<T>` component to generate OG meta tags using a HCTI template. 

#### Parameters
| Parameter               | Required | Description                                                                                                   | 
|-------------------------|:--------:|---------------------------------------------------------------------------------------------------------------|
| `TemplateId`            |    ✅     | The ID of the template to use.                                                                                |
| `TemplateValues`        |    ✅     | A JSON-serializable object that will serve as the `template_values` in the request                            |
| `TypeInfo`              |          | A `System.Text.Json.JsonTypeInfo<T>` instance that will be used to serialize `TemplateValues`                 |
| `JsonSerializerOptions` |          | A `System.Text.Json.JsonSerializerOptions` instance that will be used to serialize `TemplateValues` if provided |
| `TemplateVersion`       |          | An optional version of the template to use, if not specified, the latest version will be used                 |
| `OgMetaType`            |          | The type of meta tag to generate, such as `twitter:image`. When not specified, `og:image` will be used.       | 

#### Template Values
When providing `TemplateValues`, your object must be serializable to JSON. If you don't want to provide an existing class or create a dedicated one, a `System.Text.Json.Nodes.JsonObject` can be provided instead.

If you are using a custom class and care about performance/AOT or just want to control serialization, you can provide either a `JsonSerializerOptions` instance or `JsonTypeInfo<T>`. The `TypeInfo` parameter will be used if provided.

### `HCTI_Url_OgMetaTag`

Use the `HCTI_Url_OgMetaTag` component to generate an OG meta tag with a URL-generating image request. This tag is handy if you want to use existing styling / razor templates etc as your image source. 

#### Parameters
| Parameter      | Required | Description                                                                                                                             |
|----------------| :------: |-----------------------------------------------------------------------------------------------------------------------------------------|
| `ImageRequest` | ✅ | An instance of [`HtmlCssToImage.Models.Requests.CreateUrlImageRequest`](../HtmlCssToImage/Models/Requests/CreateUrlImageRequest.cs) |
| `OgMetaType`   | | The type of meta tag to generate, such as `twitter:image`. When not specified, `og:image` will be used.                                 |

#### Image Request
The `ImageRequest` parameter must be an instance of [`HtmlCssToImage.Models.Requests.CreateUrlImageRequest`](https://github.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/Requests/CreateUrlImageRequest.cs) . The options will be used to generate a [create-and-render request url](https://docs.htmlcsstoimage.com/getting-started/create-and-render/). All options that are provided will be URL-encoded and included in the hmac signature except for `pdf_options` which is not currently supported in create-and-render.

## Examples
Check out the [Sample Project](https://github.com/HtmlCssToImage/dotnet-client/main/src/samples/BlazorStaticServerSample) for a full example implementation.

## Other Notes
- You can use multiple tag helpers on the same page to define different meta tags like `og:image` and `twitter:image`
- You must include the `HCTIOutlet` in your `head` tag to ensure the meta tags are generated on all pages.
- It is best to keep the `TemplateValues` / `ImageRequest` parameters as simple as possible to avoid unnecessary complexity and potentially large urls.
- Your generated urls will be signed with your API Key & ID, so if you change them, your meta tags will generate different urls. 
- If you are interested in using the HCTI API directly to generate & store your images, check out the [HtmlCssToImage Client docs](https://github.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/README.md)

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities.

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).