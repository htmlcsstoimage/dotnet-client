namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Scroll through an ordered sequence of timed movements in the document or a selected container.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class RecordingScrollAction : RecordingAction
{
    /// <summary>CSS selector for a scrollable container. Omitted or null scrolls the document.</summary>
    public string? ContainerSelector { get; set; }

    /// <summary>Ordered scroll movements. At least one step is required.</summary>
    public required ScrollStep[] Steps { get; set; }

    /// <summary>Actions to run if the selected container is missing. Omitted, null, or empty skips this scroll action; it never falls back to the document. Use an error action to fail instead.</summary>
    public RecordingAction[]? IfNotFound { get; set; }
}
