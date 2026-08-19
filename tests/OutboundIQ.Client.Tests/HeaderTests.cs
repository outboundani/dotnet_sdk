using System.Reflection;

namespace OutboundIQ.Tests;

/// <summary>
/// The header set the SDK puts on every request.
/// </summary>
public class HeaderTests
{
    [Fact]
    public async Task Sends_authorization_accept_and_user_agent()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Nrm.ListAnisAsync();

        var call = handler.Single();
        Assert.Equal($"Bearer {TestClient.ApiKey}", call.Header("Authorization"));
        Assert.Equal("application/json", call.Header("Accept"));
        Assert.StartsWith("outboundiq-client/", call.Header("User-Agent"), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Omits_content_type_when_there_is_no_body()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Custom.Campaigns.GetAsync("c-1");

        Assert.False(handler.Single().HasHeader("Content-Type"));
    }

    [Fact]
    public async Task Sends_content_type_without_a_charset_when_there_is_a_body()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Assignment.NextAsync(new AssignmentRequest { ProspectPhone = "5559876543" });

        // Exactly "application/json". StringContent would append "; charset=utf-8", which the
        // TypeScript SDK does not send.
        Assert.Equal("application/json", handler.Single().Header("Content-Type"));
    }

    [Fact]
    public async Task Appends_the_user_agent_suffix_when_configured()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler, options => options.UserAgentSuffix = "acme-crm/2.1");

        await client.Nrm.ListAnisAsync();

        var userAgent = handler.Single().Header("User-Agent");
        Assert.StartsWith("outboundiq-client/", userAgent, StringComparison.Ordinal);
        Assert.EndsWith(" acme-crm/2.1", userAgent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task User_agent_carries_the_sdk_version()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var client = TestClient.Create(handler);

        await client.Nrm.ListAnisAsync();

        // The .NET mirror of the TypeScript SDK's "VERSION matches package.json" assertion: the
        // version the SDK reports, the version MSBuild computed, and the version actually sent
        // must all agree.
        var expected = typeof(EndpointSpecTests).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .First(a => a.Key == "ExpectedSdkVersion").Value;

        Assert.False(string.IsNullOrWhiteSpace(OutboundIQClient.Version));
        Assert.DoesNotContain("+", OutboundIQClient.Version, StringComparison.Ordinal);
        Assert.Equal(expected, OutboundIQClient.Version);
        Assert.Equal($"outboundiq-client/{OutboundIQClient.Version}", handler.Single().Header("User-Agent"));
    }
}
