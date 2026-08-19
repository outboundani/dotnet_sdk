using System.Net.Http;

namespace OutboundIQ.Tests;

/// <summary>
/// Produces the response for one attempt. Throwing simulates a transport failure.
/// </summary>
internal delegate Task<HttpResponseMessage> StubHandler(RecordedCall call, CancellationToken cancellationToken);

/// <summary>
/// A fake transport that records every attempt and replays scripted responses.
/// </summary>
/// <remarks>
/// This is the .NET counterpart of the TypeScript SDK's <c>fetchStub</c>. Handlers are consumed in
/// order; once they run out the last one is reused, so a single handler serves every retry.
/// </remarks>
internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly StubHandler[] _handlers;
    private readonly List<RecordedCall> _calls = [];
    // A plain object rather than System.Threading.Lock, which is .NET 9+ and would not
    // compile for the net8.0 target.
    private readonly object _gate = new();

    public StubHttpMessageHandler(params StubHandler[] handlers)
    {
        if (handlers.Length == 0)
        {
            throw new ArgumentException("At least one handler is required.", nameof(handlers));
        }

        _handlers = handlers;
    }

    /// <summary>Every attempt the SDK made, in order.</summary>
    public IReadOnlyList<RecordedCall> Calls
    {
        get
        {
            lock (_gate)
            {
                return [.. _calls];
            }
        }
    }

    /// <summary>How many attempts the SDK made.</summary>
    public int CallCount
    {
        get
        {
            lock (_gate)
            {
                return _calls.Count;
            }
        }
    }

    /// <summary>The single attempt, asserting there was exactly one.</summary>
    public RecordedCall Single()
    {
        var calls = Calls;
        Assert.Single(calls);
        return calls[0];
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var call = await RecordedCall.CaptureAsync(request, cancellationToken).ConfigureAwait(false);

        int index;
        lock (_gate)
        {
            _calls.Add(call);
            index = Math.Min(_calls.Count - 1, _handlers.Length - 1);
        }

        return await _handlers[index](call, cancellationToken).ConfigureAwait(false);
    }
}
