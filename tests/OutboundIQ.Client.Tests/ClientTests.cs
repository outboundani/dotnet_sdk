using System.Net.Http;

namespace OutboundIQ.Tests;

/// <summary>
/// Construction, configuration, and lifetime.
/// </summary>
/// <remarks>
/// The environment-variable tests mutate process-global state, so they live in one class whose
/// methods xUnit runs serially, and each restores the previous value.
/// </remarks>
public class ClientTests
{
    private const string EnvVar = OutboundIQClientOptions.ApiKeyEnvironmentVariable;

    [Fact]
    public void Exposes_every_resource()
    {
        using var client = new OutboundIQClient("oiq_test");

        Assert.NotNull(client.Assignment);
        Assert.NotNull(client.Dials);
        Assert.NotNull(client.Custom);
        Assert.NotNull(client.Custom.Campaigns);
        Assert.NotNull(client.Custom.Dispos);
        Assert.NotNull(client.Custom.Anis);
        Assert.NotNull(client.Nrm);
        Assert.NotNull(client.LiveFeed);
        Assert.NotNull(client.LiveFeed.RingCx);
        Assert.NotNull(client.AniPlanner);
    }

    [Fact]
    public void Reports_a_semver_version()
    {
        Assert.False(string.IsNullOrWhiteSpace(OutboundIQClient.Version));
        Assert.DoesNotContain("+", OutboundIQClient.Version, StringComparison.Ordinal);
        Assert.True(Version.TryParse(OutboundIQClient.Version.Split('-')[0], out _));
    }

    [Fact]
    public void Falls_back_to_the_environment_variable()
    {
        var original = Environment.GetEnvironmentVariable(EnvVar);
        try
        {
            Environment.SetEnvironmentVariable(EnvVar, "oiq_from_env");

            using var client = new OutboundIQClient();

            Assert.NotNull(client.Assignment);
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnvVar, original);
        }
    }

    [Fact]
    public void Throws_a_helpful_error_when_no_api_key_is_available()
    {
        var original = Environment.GetEnvironmentVariable(EnvVar);
        try
        {
            Environment.SetEnvironmentVariable(EnvVar, null);

            var exception = Assert.Throws<OutboundIQException>(() => new OutboundIQClient(new OutboundIQClientOptions()));

            Assert.Contains(EnvVar, exception.Message, StringComparison.Ordinal);
            Assert.Contains("Universal keys", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnvVar, original);
        }
    }

    [Fact]
    public async Task An_explicit_key_wins_over_the_environment_variable()
    {
        var original = Environment.GetEnvironmentVariable(EnvVar);
        try
        {
            Environment.SetEnvironmentVariable(EnvVar, "oiq_from_env");

            var handler = new StubHttpMessageHandler(Responses.Ok());
            using var client = TestClient.Create(handler, options => options.ApiKey = "oiq_explicit");

            await client.Nrm.ListAnisAsync();

            Assert.Equal("Bearer oiq_explicit", handler.Single().Header("Authorization"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnvVar, original);
        }
    }

    [Theory]
    [InlineData(-1, 30, 500)]
    [InlineData(2, 0, 500)]
    [InlineData(2, 30, -1)]
    public void Rejects_out_of_range_options(int maxRetries, int timeoutSeconds, int retryBaseMilliseconds)
    {
        var options = new OutboundIQClientOptions
        {
            ApiKey = "oiq_test",
            MaxRetries = maxRetries,
            Timeout = TimeSpan.FromSeconds(timeoutSeconds),
            RetryBaseDelay = TimeSpan.FromMilliseconds(retryBaseMilliseconds),
        };

        Assert.Throws<OutboundIQException>(() => new OutboundIQClient(options));
    }

    [Fact]
    public void Rejects_a_relative_base_url()
    {
        var options = new OutboundIQClientOptions
        {
            ApiKey = "oiq_test",
            BaseUrl = new Uri("/relative", UriKind.Relative),
        };

        Assert.Throws<OutboundIQException>(() => new OutboundIQClient(options));
    }

    [Fact]
    public async Task Mutating_the_options_after_construction_has_no_effect()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        var options = new OutboundIQClientOptions { ApiKey = "oiq_original" };
        using var httpClient = new HttpClient(handler);
        using var client = new OutboundIQClient(options, httpClient);

        options.ApiKey = "oiq_changed";
        await client.Nrm.ListAnisAsync();

        // Options are copied on construction, so the client cannot be reconfigured behind its back.
        Assert.Equal("Bearer oiq_original", handler.Single().Header("Authorization"));
    }

    [Fact]
    public async Task Does_not_dispose_a_caller_supplied_http_client()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok());
        using var httpClient = new HttpClient(handler);

        var client = new OutboundIQClient(new OutboundIQClientOptions { ApiKey = "oiq_test" }, httpClient);
        client.Dispose();

        // The caller owns it, so it must still be usable afterwards.
        using var response = await httpClient.GetAsync(new Uri("https://api.outboundiq.cloud/ping"));
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public void Disposing_twice_is_safe()
    {
        var client = new OutboundIQClient("oiq_test");

        client.Dispose();
        client.Dispose();
    }

    [Fact]
    public void Rejects_a_null_options_object() =>
        Assert.Throws<ArgumentNullException>(() => new OutboundIQClient((OutboundIQClientOptions)null!));

    [Fact]
    public async Task Resource_methods_reject_null_arguments()
    {
        using var client = new OutboundIQClient("oiq_test");

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.Assignment.NextAsync(null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.Dials.CreateAsync(null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.Custom.Campaigns.CreateAsync(null!));
        await Assert.ThrowsAsync<ArgumentException>(() => client.Custom.Campaigns.GetAsync(string.Empty));
    }
}
