using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Benchmarks.Benchmarks;
[MemoryDiagnoser]
public class HmacBenchmark
{
    private const string Message = "url=https%3A%2F%2Fgoogle.com%3Fabc%3D123&viewport_width=1200&viewport_height=630";
    private const string Secret = "sk_test_51MzByzL9xVzVzVzVzVzVzVzV";
    [Benchmark(Baseline = true)]
    public string Hmac_Standard_Instance()
    {
        // The common, allocation-heavy way
        var keyBytes = Encoding.UTF8.GetBytes(Secret);
        var messageBytes = Encoding.UTF8.GetBytes(Message);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(messageBytes);


        return Convert.ToHexStringLower(hash);
    }

    [Benchmark]
    public string HCTI_HmacToken_Optimized()
    {

        return HmacToken.CreateToken(Message, Secret);
    }
}