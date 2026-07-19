using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Tests;

public class QueryStringBuilderTests
{
    [Fact]
    public void QueryString_KeepsLiteralPrefixOutOfQuery()
    {
        QueryStringBuilder builder = new(stackalloc char[8]);
        try
        {
            builder.AppendLiteral("https://example.test/path");
            builder.EncodeSafeKeyValue("width", "100");
            builder.Encode("unsafe key", "a&b");

            Assert.Equal(
                "https://example.test/path?width=100&unsafe%20key=a%26b",
                builder.FullSpan.ToString());
            Assert.Equal(
                "?width=100&unsafe%20key=a%26b",
                builder.QueryString(true).ToString());
            Assert.Equal(
                "width=100&unsafe%20key=a%26b",
                builder.QueryString(false).ToString());
        }
        finally
        {
            builder.Dispose();
        }
    }

    [Fact]
    public void QueryString_WhenNoParameters_IsEmpty()
    {
        QueryStringBuilder builder = new(stackalloc char[8]);
        try
        {
            builder.AppendLiteral("https://example.test/path");

            Assert.Equal("https://example.test/path", builder.FullSpan.ToString());
            Assert.True(builder.QueryString(true).IsEmpty);
            Assert.True(builder.QueryString(false).IsEmpty);
        }
        finally
        {
            builder.Dispose();
        }
    }

    [Fact]
    public void EncodeSafeKey_GrowsForEscapedValue()
    {
        QueryStringBuilder builder = new(stackalloc char[8]);
        try
        {
            builder.EncodeSafeKey("x", "\u0800");

            Assert.Equal("?x=%E0%A0%80", builder.FullSpan.ToString());
        }
        finally
        {
            builder.Dispose();
        }
    }

    [Fact]
    public void Encode_GrowsForEscapedKeyAndValue()
    {
        QueryStringBuilder builder = new(stackalloc char[8]);
        try
        {
            builder.Encode("\u0800", "\u0800");

            Assert.Equal("?%E0%A0%80=%E0%A0%80", builder.FullSpan.ToString());
        }
        finally
        {
            builder.Dispose();
        }
    }
}
