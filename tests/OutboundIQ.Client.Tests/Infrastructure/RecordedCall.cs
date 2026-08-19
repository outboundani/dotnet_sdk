using System.Net.Http;

namespace OutboundIQ.Tests;

/// <summary>
/// Everything the SDK put on the wire for one attempt.
/// </summary>
internal sealed record RecordedCall(
    string Method,
    string Url,
    IReadOnlyDictionary<string, string> Headers,
    string? Body)
{
    public static async Task<RecordedCall> CaptureAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Request headers and content headers are separate collections, and Content-Type lives on
        // the latter. Merging them is what lets a test assert that Content-Type is absent on a
        // bodyless request rather than passing for the wrong reason.
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var header in request.Headers)
        {
            headers[header.Key] = string.Join(", ", header.Value);
        }

        string? body = null;
        if (request.Content is not null)
        {
            foreach (var header in request.Content.Headers)
            {
                headers[header.Key] = string.Join(", ", header.Value);
            }

            // Buffered here because this is the last point at which the content is readable.
            body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        return new RecordedCall(
            request.Method.Method,
            // AbsoluteUri, not ToString(): ToString() unescapes percent-encoding and would
            // hide whether the SDK escaped a query value correctly.
            request.RequestUri?.AbsoluteUri ?? string.Empty,
            headers,
            body);
    }

    public string? Header(string name) => Headers.TryGetValue(name, out var value) ? value : null;

    public bool HasHeader(string name) => Headers.ContainsKey(name);
}
