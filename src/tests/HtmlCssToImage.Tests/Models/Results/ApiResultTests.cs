using System.Net;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage.Tests.Models.Results;

public class ApiResultTests
{
    [Fact]
    public void Dispose_DisposesOwnedHttpResponseMessage()
    {
        var content = new TrackingHttpContent();
        var response = new HttpResponseMessage
        {
            Content = content
        };
        var result = new ApiResult<object?>
        {
            HttpResponseMessage = response
        };

        result.Dispose();

        Assert.True(content.IsDisposed);
    }

    [Fact]
    public void Dispose_CanBeCalledMoreThanOnce()
    {
        var content = new TrackingHttpContent();
        var result = new ApiResult<object?>
        {
            HttpResponseMessage = new HttpResponseMessage
            {
                Content = content
            }
        };

        result.Dispose();
        result.Dispose();

        Assert.Equal(1, content.DisposeCount);
    }

    private sealed class TrackingHttpContent : HttpContent
    {
        public bool IsDisposed => DisposeCount > 0;

        public int DisposeCount { get; private set; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            Task.CompletedTask;

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCount++;
            }

            base.Dispose(disposing);
        }
    }
}
