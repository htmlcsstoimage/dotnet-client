namespace HtmlCssToImage.Models.Requests;

/// <summary>A rule for blocking matching browser requests while an image is rendered.</summary>
public sealed class RequestOverride
{
    /// <summary>The action to take. Currently only <see cref="RequestOverrideAction.Block"/> is supported.</summary>
    public RequestOverrideAction Action { get; set; } = RequestOverrideAction.Block;

    /// <summary>An optional URL pattern with * wildcards. At least one matcher must be supplied.</summary>
    public string? Url { get; set; }

    /// <summary>Optional browser resource types. At least one matcher must be supplied.</summary>
    public RequestOverrideResourceType[]? ResourceTypes { get; set; }
}
