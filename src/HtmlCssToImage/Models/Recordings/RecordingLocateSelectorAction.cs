namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Look for a matching element and execute the corresponding branch. Branch actions supply their own selectors.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class RecordingLocateSelectorAction : RecordingAction
{
    /// <summary>CSS selector of the element to find.</summary>
    public required string Selector { get; set; }

    /// <summary>Maximum wait for the element in milliseconds. Defaults to 0, which performs a single check. The recording deadline can end the wait earlier.</summary>
    public uint? TimeoutMs { get; set; }

    /// <summary>Actions to run when the element is found. Omitted, null, or empty continues. Actions do not inherit the lookup's selector.</summary>
    public RecordingAction[]? IfFound { get; set; }

    /// <summary>Actions to run when the element is not found before timeout. Omitted, null, or empty continues. Use an error action to fail instead.</summary>
    public RecordingAction[]? IfNotFound { get; set; }
}
