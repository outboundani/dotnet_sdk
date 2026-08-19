namespace OutboundIQ.Tests;

/// <summary>
/// Per-attempt timeouts, and the distinction between a timeout and caller cancellation.
/// </summary>
public class TimeoutAndCancellationTests
{
    [Fact]
    public async Task A_hanging_request_times_out_as_a_connection_error()
    {
        var handler = new StubHttpMessageHandler(Responses.Hanging());
        using var client = TestClient.Create(handler, options =>
        {
            options.Timeout = TimeSpan.FromMilliseconds(50);
            options.MaxRetries = 0;
        });

        var exception = await Assert.ThrowsAsync<OutboundIQConnectionException>(() => client.Nrm.ListAnisAsync());

        Assert.Contains("timed out", exception.Message, StringComparison.Ordinal);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task The_timeout_applies_per_attempt_not_per_call()
    {
        var handler = new StubHttpMessageHandler(Responses.Hanging());
        using var client = TestClient.Create(handler, options => options.Timeout = TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAsync<OutboundIQConnectionException>(() => client.Nrm.ListAnisAsync());

        // Each attempt gets its own fresh timeout, so a retried GET times out three times over.
        Assert.Equal(3, handler.CallCount);
    }

    [Fact]
    public async Task Caller_cancellation_throws_OperationCanceledException_not_a_connection_error()
    {
        var handler = new StubHttpMessageHandler(Responses.Hanging());
        using var client = TestClient.Create(handler);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        // The caller's own cancellation must propagate untouched. Converting it into an SDK
        // exception would break `catch (OperationCanceledException)` in consumer code.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.Nrm.ListAnisAsync(cancellationToken: cancellation.Token));
    }

    [Fact]
    public async Task Cancellation_during_backoff_propagates()
    {
        using var cancellation = new CancellationTokenSource();

        var handler = new StubHttpMessageHandler((_, _) =>
        {
            // Cancel while the SDK is about to wait out its backoff.
            cancellation.Cancel();
            return Responses.Json(500, "{}")(null!, default);
        });

        using var client = TestClient.Create(handler, options => options.RetryBaseDelay = TimeSpan.FromSeconds(30));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.Nrm.ListAnisAsync(cancellationToken: cancellation.Token));

        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task A_completed_request_is_unaffected_by_the_timeout()
    {
        var handler = new StubHttpMessageHandler(Responses.Ok("""{"result":"success"}"""));
        using var client = TestClient.Create(handler, options => options.Timeout = TimeSpan.FromMilliseconds(50));

        var result = await client.Nrm.ListAnisAsync();

        Assert.Equal("success", result.Result);
    }
}
