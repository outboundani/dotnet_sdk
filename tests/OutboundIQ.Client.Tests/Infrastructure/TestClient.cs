using System.Net.Http;

namespace OutboundIQ.Tests;

/// <summary>
/// Builds a client wired to a stub transport, with backoff shortened so retry tests stay fast.
/// </summary>
internal static class TestClient
{
    public const string ApiKey = "oiq_test_key";
    public const string BaseUrl = "https://api.outboundiq.cloud";

    public static OutboundIQClient Create(
        StubHttpMessageHandler handler,
        Action<OutboundIQClientOptions>? configure = null)
    {
        var options = new OutboundIQClientOptions
        {
            ApiKey = ApiKey,
            RetryBaseDelay = TimeSpan.FromMilliseconds(1),
        };

        configure?.Invoke(options);

        var httpClient = new HttpClient(handler, disposeHandler: true)
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };

        return new OutboundIQClient(options, httpClient);
    }
}
