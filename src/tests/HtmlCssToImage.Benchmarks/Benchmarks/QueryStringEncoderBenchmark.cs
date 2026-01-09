using System.Text;
using System.Web;
using BenchmarkDotNet.Attributes;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;

namespace HtmlCssToImage.Benchmarks.Benchmarks;
[MemoryDiagnoser]
public class QueryStringEncoderBenchmark
{
    private readonly CreateUrlImageRequest _request = new()
    {
        Url = "https://example.com/blog/post-1?utm_source=twitter&campaign=winter_sale&emoji=💯",
        Css = "body { background: #f0f0f0; } .hero { font-family: 'Open Sans'; }",
        ViewportWidth = 1200,
        ViewportHeight = 630,
        DeviceScale = 2.5,
        MsDelay = 500,
        FullScreen = true,
        ColorScheme = ColorSchemeType.dark,
        Timezone = "America/New_York"
    };


    [Benchmark(Baseline = true)]
    public string BuiltIn_HttpUtility()
    {
        var sb = new StringBuilder();
        sb.Append("url=").Append(HttpUtility.UrlEncode(_request.Url));
        sb.Append("&css=").Append(HttpUtility.UrlEncode(_request.Css));
        sb.Append("&viewport_width=").Append(_request.ViewportWidth);
        sb.Append("&viewport_height=").Append(_request.ViewportHeight);
        sb.Append("&device_scale=").Append(_request.DeviceScale);
        sb.Append("&ms_delay=").Append(_request.MsDelay);
        sb.Append("&full_screen=true");
        sb.Append("&color_scheme=").Append(_request.ColorScheme?.ToString());
        sb.Append("&timezone=").Append(HttpUtility.UrlEncode(_request.Timezone));
        return sb.ToString();
    }


    [Benchmark]
    public string HCTI_QueryStringEncoder()
    {
        ArrayOrSpan<char> chars = new(stackalloc char[512]);
        try
        {
            HtmlCssToImageClient.CreateAndRenderUrlQueryString(_request, ref chars);

            var result = new string(chars.LimitedSpan);


            return result;
        }
        finally
        {
            chars.Dispose();
        }

    }
}