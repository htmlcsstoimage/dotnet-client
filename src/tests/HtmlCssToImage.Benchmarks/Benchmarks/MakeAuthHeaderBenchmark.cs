using System.Text;
using BenchmarkDotNet.Attributes;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;

namespace HtmlCssToImage.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class MakeAuthHeaderBenchmark
{
    private readonly HtmlCssToImageOptions _options = new()
    {
        ApiId = "user_1234567890",
        ApiKey = "761882d2-8f9d-4e2a-9e1e-c76a147823f6"
    };

    [Benchmark(Baseline = true)]
    public string AuthHeader_Standard_StringFormat()
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ApiId}:{_options.ApiKey}"));
    }

    [Benchmark]
    public string HCTI_AuthHeader_Optimized()
    {
        return _options.AuthHeader();
    }

}