using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Models.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Components.Web;

namespace HtmlCssToImage.Blazor;

public abstract class HCTI_OgMetaTagBase:ComponentBase
{
    [Inject] public IHtmlCssToImageClient HtmlCssToImageClient { get; set; } = null!;

    [Parameter] public string? OgMetaType { get; set; }

    protected string? MetaUrl { get; set; }

    protected abstract void SetMetaUrl();

    protected override void OnInitialized()
    {
        SetMetaUrl();
        base.OnInitialized();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (MetaUrl != null)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionId), HCTIOutlet.SectionId(OgMetaType ?? "og:image"));
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildMetaRenderTree);
            builder.CloseComponent();
        }
        base.BuildRenderTree(builder);
    }

    private void BuildMetaRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "meta");
        builder.AddAttribute(1, "name", OgMetaType??"og:image");
        builder.AddAttribute(2, "content", MetaUrl);
        builder.CloseElement();
    }
}