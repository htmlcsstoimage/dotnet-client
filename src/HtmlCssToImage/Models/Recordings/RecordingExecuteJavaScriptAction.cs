namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Execute JavaScript in the page while recording continues, awaiting any returned promise.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class RecordingExecuteJavaScriptAction : RecordingAction
{
    /// <summary>JavaScript async function body. Supports await; its return value is ignored. For example: await new Promise(resolve =&gt; setTimeout(resolve, 500));</summary>
    public required string Script { get; set; }

    /// <summary>Maximum execution time in milliseconds. Zero or omitted uses the page timeout. Maximum 90000. Recording time and wall time are both bounded.</summary>
    public uint? TimeoutMs { get; set; }

    /// <summary>Actions to run if the script throws or times out. Null or omitted fails the recording; an empty array ignores the error. A timeout cannot undo side effects or cancel promises and timers already created by the script.</summary>
    public RecordingAction[]? IfError { get; set; }
}
