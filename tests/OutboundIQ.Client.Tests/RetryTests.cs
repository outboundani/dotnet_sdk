namespace OutboundIQ.Tests;

/// <summary>
/// The retry policy, which must match the TypeScript SDK exactly.
/// </summary>
/// <remarks>
/// The asymmetry is deliberate and is the whole point of these tests: GET is safe to repeat, but a
/// POST, PUT, or DELETE that failed mid-flight may already have been applied. Retrying those on a
/// network error or a 5xx could record a dial twice. Only 429 is safe for them, because the API
/// rejected the request before processing it.
/// </remarks>
public class RetryTests
{
    private static Task Invoke(IOutboundIQClient client, string verb) => verb switch
    {
        "GET" => client.Nrm.ListAnisAsync(),
        "POST" => client.Assignment.NextAsync(new AssignmentRequest { ProspectPhone = "5559876543" }),
        "PUT" => client.Custom.Campaigns.UpdateAsync(new CustomCampaignUpdate { Id = "c-1" }),
        "DELETE" => client.Custom.Campaigns.DeleteAsync("c-1"),
        _ => throw new ArgumentOutOfRangeException(nameof(verb)),
    };

    private static StubHandler Failure(string kind) => kind switch
    {
        "network" => Responses.NetworkError(),
        "500" => Responses.Json(500, """{"message":"boom"}"""),
        "429" => Responses.Json(429, """{"message":"slow down"}"""),
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    [Theory]
    // GET is retry-safe on every transient failure.
    [InlineData("GET", "network", 3)]
    [InlineData("GET", "500", 3)]
    [InlineData("GET", "429", 3)]
    // Writes are retried only on 429.
    [InlineData("POST", "network", 1)]
    [InlineData("POST", "500", 1)]
    [InlineData("POST", "429", 3)]
    [InlineData("PUT", "network", 1)]
    [InlineData("PUT", "500", 1)]
    [InlineData("PUT", "429", 3)]
    [InlineData("DELETE", "network", 1)]
    [InlineData("DELETE", "500", 1)]
    [InlineData("DELETE", "429", 3)]
    public async Task Retries_follow_the_method_and_failure_matrix(string verb, string failure, int expectedAttempts)
    {
        var handler = new StubHttpMessageHandler(Failure(failure));
        using var client = TestClient.Create(handler);

        await Assert.ThrowsAnyAsync<OutboundIQException>(() => Invoke(client, verb));

        Assert.Equal(expectedAttempts, handler.CallCount);
    }

    [Theory]
    [InlineData("network", typeof(OutboundIQConnectionException))]
    [InlineData("500", typeof(OutboundIQApiException))]
    [InlineData("429", typeof(OutboundIQRateLimitException))]
    public async Task Surfaces_the_right_exception_once_retries_are_exhausted(string failure, Type expected)
    {
        var handler = new StubHttpMessageHandler(Failure(failure));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAnyAsync<OutboundIQException>(() => client.Nrm.ListAnisAsync());

        Assert.IsType(expected, exception);
    }

    [Fact]
    public async Task Succeeds_when_a_retry_succeeds()
    {
        var handler = new StubHttpMessageHandler(
            Responses.NetworkError(),
            Responses.Json(500, "{}"),
            Responses.Ok("""{"result":"success","count":1}"""));

        using var client = TestClient.Create(handler);

        var result = await client.Nrm.ListAnisAsync();

        Assert.Equal(3, handler.CallCount);
        Assert.Equal("success", result.Result);
    }

    [Fact]
    public async Task MaxRetries_zero_disables_retries()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(500, "{}"));
        using var client = TestClient.Create(handler, options => options.MaxRetries = 0);

        await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task Resends_the_body_on_every_attempt()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(429, "{}"));
        using var client = TestClient.Create(handler);

        await Assert.ThrowsAsync<OutboundIQRateLimitException>(
            () => client.Assignment.NextAsync(new AssignmentRequest { ProspectPhone = "5559876543" }));

        // A consumed HttpContent cannot be resent, so every attempt must carry the full body.
        Assert.Equal(3, handler.CallCount);
        Assert.All(handler.Calls, call => JsonAssert.Equivalent("""{"prospect_phone":"5559876543"}""", call.Body));
    }

    [Fact]
    public async Task Surfaces_retry_after_seconds_on_the_rate_limit_exception()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(429, "{}", ("Retry-After", "0")));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQRateLimitException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal(TimeSpan.Zero, exception.RetryAfter);
        Assert.Equal(3, handler.CallCount);
        Assert.Equal(429, exception.Status);
    }

    [Fact]
    public async Task Ignores_retry_after_in_http_date_form()
    {
        // The TypeScript SDK parses Retry-After only as a number of seconds. Matching that keeps
        // backoff identical across the two SDKs.
        var handler = new StubHttpMessageHandler(
            Responses.Json(429, "{}", ("Retry-After", "Wed, 21 Oct 2026 07:28:00 GMT")));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQRateLimitException>(() => client.Nrm.ListAnisAsync());

        Assert.Null(exception.RetryAfter);
        Assert.Equal(3, handler.CallCount);
    }

    [Fact]
    public async Task Ignores_a_negative_retry_after()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(429, "{}", ("Retry-After", "-5")));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQRateLimitException>(() => client.Nrm.ListAnisAsync());

        Assert.Null(exception.RetryAfter);
    }

    [Fact]
    public async Task Does_not_retry_a_4xx_that_is_not_429()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(404, """{"message":"not found"}"""));
        using var client = TestClient.Create(handler);

        await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal(1, handler.CallCount);
    }
}
