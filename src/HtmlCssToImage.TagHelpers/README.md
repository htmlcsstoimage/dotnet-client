# [HTML/CSS to Image](https://htmlcsstoimage.com/) 
## .NET / C# Client - Razor Tag Helpers

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180-white.png">
  <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
  <img alt="HCTI Logo" src="https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/logo-180x180.png">
</picture>

This package provides Razor Tags for generating Open Graph meta tags using the HtmlCssToImage client.

[![NuGet Version](https://img.shields.io/nuget/v/htmlcsstoimage.taghelpers)](https://www.nuget.org/packages/HtmlCssToImage.TagHelpers)

## Getting Started

### Installation

Add the package to your project:

```bash
dotnet add package HtmlCssToImage.TagHelpers
```

### Configuration

You'll also need to register the `IHtmlCssToImageClient` in your application's startup (usually `Program.cs` or `Startup.cs`):

```csharp
builder.Services.AddHtmlCssToImage("api_id", "api_key");
```

Refer to the [HtmlCssToImage.DependencyInjection docs](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage.DependencyInjection/README.md) for more details on configuring the client.

### Tag Helper Reference

Finally, you need to add the tag helpers to your `_ViewImports.cshtml` file:
```razor
@addTagHelper *, HtmlCssToImage.TagHelpers
```

### Setup

In your `_Layout.cshtml` you'll want to add a `Section` to your `head` tag to include the generated meta tags:
```razor
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>@ViewData["Title"] - RazorPagesSample</title>
    @await RenderSectionAsync("HCTIMetaTags", required: false)
</head>
```

and in each page you want to generate an image for, add a `Section` like so: 
```razor
@section HCTIMetaTags {
    <hcti-og-templated template-id=..... />
    }
```

> [!TIP]
> If you already have a Section for meta tags in your layout, it's fine to use the tag helpers there, they don't require a dedicated section.

## Tag Helpers

There are 2 available tag helpers, to inject `meta` tags for your pages.

### `hcti-og-templated`

Use the `<hcti-og-templated>` tag to generate OG meta tags using a HCTI template. 

#### Parameters
|Parameter |  Required  | Description                                                                         | 
|----------|:----------:|------------------|
| `template-id` |     ✅      | The ID of the template to use.                                                      |
| `template-values` |     ✅      | A JSON-serializable object that will serve as the `template_values` in the request. |
| `json-options` |            | A `System.Text.Json.JsonSerializerOptions` instance that will be used to serialize `template-values` |
| `template-version` |            | An optional version of the template to use, if not specified, the latest version will be used |
| `og-meta-type` | | The type of meta tag to generate, such as `twitter:image`. When not specified, `og:image` will be used. | 

#### Template Values
When providing `template-values`, your object must be serializable to JSON. If you don't want to provide an existing class or create a dedicated one, a `System.Text.Json.Nodes.JsonObject` can be provided instead.

### `hcti-og-url`

Use the `<hcti-og-url>` tag to generate an OG meta tag with a URL-generating image request. This tag is handy if you want to use existing styling / razor templates etc as your image source. 

#### Parameters
| Parameter | Required | Description                                                                                                                             |
| --------- | :------: |-----------------------------------------------------------------------------------------------------------------------------------------|
| `image-request` | ✅ | An instance of [`HtmlCssToImage.Models.Requests.CreateUrlImageRequest`](../HtmlCssToImage/Models/Requests/CreateUrlImageRequest.cs) |
| `og-meta-type` | | The type of meta tag to generate, such as `twitter:image`. When not specified, `og:image` will be used.                                 |

#### Image Request
The `image-request` parameter must be an instance of [`HtmlCssToImage.Models.Requests.CreateUrlImageRequest`](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/Models/Requests/CreateUrlImageRequest.cs) . The options will be used to generate a [create-and-render request url](https://docs.htmlcsstoimage.com/getting-started/create-and-render/). All options that are provided will be URL-encoded and included in the hmac signature except for `pdf_options` which is not currently supported in create-and-render.

## Examples
Check out the [Sample Project](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/samples/RazorPagesSample) for a full example implementation.

## Other Notes
- You can use multiple tag helpers on the same page to define different meta tags like `og:image` and `twitter:image`
- It is best to keep the `template-values` / `image-request` parameters as simple as possible to avoid unnecessary complexity and potentially large urls.
- Your generated urls will be signed with your API Key & ID, so if you change them, your meta tags will generate different urls.
- If you are interested in using the HCTI API directly to generate & store your images, check out the [HtmlCssToImage Client docs](https://raw.githubusercontent.com/HtmlCssToImage/dotnet-client/main/src/HtmlCssToImage/README.md)

---

> [!IMPORTANT]
> Check out the [HTML/CSS To Image Docs](https://docs.htmlcsstoimage.com) for more details on the API's capabilities.

> [!TIP]
> Get started for free at [htmlcsstoimage.com](https://htmlcsstoimage.com).