using System.Net.Http;
using System.Text.Json;

namespace OutboundIQ;

/// <summary>
/// A response body read leniently: valid JSON is parsed, anything else is kept as text.
/// </summary>
/// <remarks>
/// The API is not consistent about error shapes — <c>{"success":false,"message":"..."}</c> is the
/// common one, but plain text and HTML come back from infrastructure layers. Nothing here throws
/// on a malformed body; the caller decides what to do.
/// </remarks>
internal readonly struct ResponseBody
{
    private ResponseBody(string? raw, JsonElement? json, string? message)
    {
        Raw = raw;
        Json = json;
        Message = message;
    }

    /// <summary>The body exactly as received, or <see langword="null"/> when it was empty.</summary>
    public string? Raw { get; }

    /// <summary>The body parsed as JSON, when it was valid JSON.</summary>
    public JsonElement? Json { get; }

    /// <summary>The <c>message</c> property, when the body was a JSON object carrying one.</summary>
    public string? Message { get; }

    public static async Task<ResponseBody> ReadAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
#if NET8_0_OR_GREATER
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        return Parse(text);
    }

    public static ResponseBody Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return default;
        }

        try
        {
            using var document = JsonDocument.Parse(text!);

            // Clone detaches the element from the document, which is disposed on the way out.
            var root = document.RootElement.Clone();

            string? message = null;
            if (root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("message", out var messageProperty)
                && messageProperty.ValueKind == JsonValueKind.String)
            {
                message = messageProperty.GetString();
            }

            return new ResponseBody(text, root, message);
        }
        catch (JsonException)
        {
            // Not JSON. Keep the text so the caller can still see what came back.
            return new ResponseBody(text, json: null, message: null);
        }
    }
}
