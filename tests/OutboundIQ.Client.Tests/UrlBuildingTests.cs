namespace OutboundIQ.Tests;

/// <summary>
/// How request URLs are assembled from the base address, path, and query parameters.
/// </summary>
public class UrlBuildingTests
{
    [Fact]
    public async Task Omits_query_parameters_that_are_not_set()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Nrm.ListAnisAsync(new NrmListAnisParams { Page = 3 });

        Assert.Equal($"{TestClient.BaseUrl}/nrm/anis?page=3", handler.Single().Url);
    }

    [Fact]
    public async Task Includes_every_parameter_that_is_set()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Nrm.ListAnisAsync(new NrmListAnisParams { Page = 2, PageSize = 50, Number = "555" });

        Assert.Equal($"{TestClient.BaseUrl}/nrm/anis?page=2&page_size=50&number=555", handler.Single().Url);
    }

    [Theory]
    [InlineData("https://api.outboundiq.cloud")]
    [InlineData("https://api.outboundiq.cloud/")]
    [InlineData("https://api.outboundiq.cloud///")]
    public async Task Strips_trailing_slashes_from_the_base_url(string baseUrl)
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler, options => options.BaseUrl = new Uri(baseUrl));

        await client.Nrm.ListAnisAsync();

        Assert.Equal("https://api.outboundiq.cloud/nrm/anis", handler.Single().Url);
    }

    [Fact]
    public async Task Escapes_query_parameter_values()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Custom.Campaigns.GetAsync("a b&c");

        Assert.Equal($"{TestClient.BaseUrl}/custom/campaigns?id=a%20b%26c", handler.Single().Url);
    }

    [Fact]
    public async Task Honors_a_custom_base_url()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler, options => options.BaseUrl = new Uri("https://staging.example.com/v2"));

        await client.Nrm.ListAnisAsync();

        Assert.Equal("https://staging.example.com/v2/nrm/anis", handler.Single().Url);
    }
}
