using System.Net;
using System.Net.Http;
using System.Text;

namespace OutboundIQ.Tests;

/// <summary>
/// Scripted responses for <see cref="StubHttpMessageHandler"/>.
/// </summary>
internal static class Responses
{
    /// <summary>A JSON response with the given status code.</summary>
    public static StubHandler Json(int status, string json, params (string Name, string Value)[] headers) =>
        (_, _) =>
        {
            var response = new HttpResponseMessage((HttpStatusCode)status)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            foreach (var (name, value) in headers)
            {
                response.Headers.TryAddWithoutValidation(name, value);
            }

            return Task.FromResult(response);
        };

    /// <summary>A 200 response carrying <c>{"success":true}</c>.</summary>
    public static StubHandler Ok(string json = """{"success":true}""") => Json(200, json);

    /// <summary>A response with a body that is not JSON.</summary>
    public static StubHandler Text(int status, string text) =>
        (_, _) => Task.FromResult(new HttpResponseMessage((HttpStatusCode)status)
        {
            Content = new StringContent(text, Encoding.UTF8, "text/plain"),
        });

    /// <summary>A response with no body at all.</summary>
    public static StubHandler Empty(int status) =>
        (_, _) => Task.FromResult(new HttpResponseMessage((HttpStatusCode)status)
        {
            Content = new StringContent(string.Empty),
        });

    /// <summary>A transport failure, as if the connection dropped.</summary>
    public static StubHandler NetworkError(string message = "socket hang up") =>
        (_, _) => throw new HttpRequestException(message);

    /// <summary>A request that never completes, so the per-attempt timeout fires.</summary>
    public static StubHandler Hanging() =>
        async (_, cancellationToken) =>
        {
            await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
            throw new UnreachableException();
        };

    private sealed class UnreachableException : Exception;
}
