using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HtmlCssToImage.TagHelpers;

public abstract class HCTI_OgMetaTagHelperBase : TagHelper
{
    protected readonly IHtmlCssToImageClient _htmlCssToImageClient;

    public HCTI_OgMetaTagHelperBase(IHtmlCssToImageClient htmlCssToImageClient)
    {
        _htmlCssToImageClient = htmlCssToImageClient;
    }

    [HtmlAttributeName("og-meta-type")] public string? OgMetaType { get; set; }

    protected string? MetaUrl { get; set; }

    protected abstract void SetMetaUrl();

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        SetMetaUrl();
        if (String.IsNullOrEmpty(MetaUrl))
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = "meta";
        output.Attributes.SetAttribute("name", OgMetaType ?? "og:image");
        output.Attributes.SetAttribute("content", MetaUrl);
        output.TagMode = TagMode.SelfClosing;
    }
}