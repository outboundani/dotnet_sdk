using System.Text.Json;

namespace OutboundIQ.Tests;

/// <summary>
/// How HTTP status codes and response bodies become exceptions.
/// </summary>
public class ErrorMappingTests
{
    [Theory]
    [InlineData(401)]
    [InlineData(403)]
    public async Task Auth_failures_map_to_an_authentication_exception(int status)
    {
        var handler = new StubHttpMessageHandler(Responses.Json(status, """{"success":false,"message":"Missing API key"}"""));
        using var client = TestClient.Create(handler, options => options.MaxRetries = 0);

        var exception = await Assert.ThrowsAsync<OutboundIQAuthenticationException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal(status, exception.Status);
        Assert.Contains("Missing API key", exception.Message, StringComparison.Ordinal);
        Assert.Contains("universal", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_authentication_exception_is_catchable_as_an_api_exception()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(401, "{}"));
        using var client = TestClient.Create(handler);

        // Unlike the TypeScript SDK, where these are siblings, the .NET hierarchy nests them so a
        // single catch handles every HTTP error.
        await Assert.ThrowsAnyAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());
    }

    [Fact]
    public async Task Other_failures_map_to_an_api_exception_carrying_status_and_body()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(404, """{"success":false,"message":"not found"}"""));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal(404, exception.Status);
        Assert.Contains("404", exception.Message, StringComparison.Ordinal);
        Assert.Contains("not found", exception.Message, StringComparison.Ordinal);
        Assert.Contains("GET /nrm/anis", exception.Message, StringComparison.Ordinal);

        Assert.NotNull(exception.Body);
        Assert.Equal("not found", exception.Body!.Value.GetProperty("message").GetString());
        Assert.Contains("not found", exception.RawBody!, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_non_json_error_body_is_preserved_as_text()
    {
        var handler = new StubHttpMessageHandler(Responses.Text(502, "<html>Bad Gateway</html>"));
        using var client = TestClient.Create(handler, options => options.MaxRetries = 0);

        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal("<html>Bad Gateway</html>", exception.RawBody);
        Assert.Null(exception.Body);
    }

    [Fact]
    public async Task An_empty_error_body_leaves_the_body_unset()
    {
        var handler = new StubHttpMessageHandler(Responses.Empty(400));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Null(exception.Body);
        Assert.Equal(400, exception.Status);
    }

    [Fact]
    public async Task Captures_the_request_id_and_includes_it_in_the_message()
    {
        var handler = new StubHttpMessageHandler(
            Responses.Json(400, """{"message":"bad request"}""", ("x-request-id", "req-abc-123")));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Equal("req-abc-123", exception.RequestId);
        Assert.Contains("req-abc-123", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Request_id_capture_can_be_disabled()
    {
        var handler = new StubHttpMessageHandler(Responses.Json(400, "{}", ("x-request-id", "req-abc-123")));
        using var client = TestClient.Create(handler, options => options.CaptureRequestId = false);

        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Null(exception.RequestId);
        Assert.DoesNotContain("req-abc-123", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_empty_success_body_is_an_error()
    {
        var handler = new StubHttpMessageHandler(Responses.Empty(200));
        using var client = TestClient.Create(handler);

        // C# cannot return "undefined" into a non-nullable T, so an empty 200 is surfaced rather
        // than silently handed back as null.
        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Contains("empty body", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_malformed_success_body_is_an_error()
    {
        var handler = new StubHttpMessageHandler(Responses.Text(200, "not json at all"));
        using var client = TestClient.Create(handler);

        var exception = await Assert.ThrowsAsync<OutboundIQApiException>(() => client.Nrm.ListAnisAsync());

        Assert.Contains("could not be parsed", exception.Message, StringComparison.Ordinal);
        Assert.Equal("not json at all", exception.RawBody);
        Assert.IsType<JsonException>(exception.InnerException);
    }

    [Fact]
    public async Task A_network_failure_maps_to_a_connection_exception()
    {
        var handler = new StubHttpMessageHandler(Responses.NetworkError());
        using var client = TestClient.Create(handler, options => options.MaxRetries = 0);

        var exception = await Assert.ThrowsAsync<OutboundIQConnectionException>(() => client.Nrm.ListAnisAsync());

        Assert.Contains("Network error", exception.Message, StringComparison.Ordinal);
        Assert.Null(exception.Status);
    }
}
