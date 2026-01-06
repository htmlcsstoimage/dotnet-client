using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Models.Requests;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HtmlCssToImage.TagHelpers;

[HtmlTargetElement("hcti-og-url", TagStructure = TagStructure.WithoutEndTag)]
public class HCTI_Url_OgMetaTagHelper:HCTI_OgMetaTagHelperBase
{
    public HCTI_Url_OgMetaTagHelper(IHtmlCssToImageClient htmlCssToImageClient) : base(htmlCssToImageClient)
    {

    }

    [HtmlAttributeName("image-request")]
    public CreateUrlImageRequest ImageRequest { get; set; } = null!;


    protected override void SetMetaUrl()
    {
         MetaUrl= _htmlCssToImageClient.CreateAndRenderUrl(ImageRequest);
    }

}