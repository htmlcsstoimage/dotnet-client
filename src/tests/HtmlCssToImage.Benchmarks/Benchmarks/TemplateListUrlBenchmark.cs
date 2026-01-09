using BenchmarkDotNet.Attributes;

namespace HtmlCssToImage.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class TemplateListUrlBenchmark
{
    private const string ID = "t-daa9c953-6ff0-4492-b301-66fa6f72efe6";
    private const uint count = 10;

    public IEnumerable<object?[]> UrlScenarios()
    {
        yield return new object?[] { null, null };               // All templates, first page
        yield return new object?[] { ID, null };         // Specific template, first page
        yield return new object?[] { ID, 1735689600L };  // Specific template, next page
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(UrlScenarios))]
    public string Basic(string? templateId, long? nextPageStart)
    {
        var url = $"{HtmlCssToImageClient.TEMPLATE_BASE_URL}{(string.IsNullOrEmpty(templateId)?"":$"/{templateId}")}?count={count}&max_version={nextPageStart ?? long.MaxValue}";
        return url;
    }

    [Benchmark]
    [ArgumentsSource(nameof(UrlScenarios))]
    public string Optimized(string? templateId, long? nextPageStart)
    {
        return HtmlCssToImageClient.GetTemplateListUrl(templateId, count, nextPageStart);
    }
}