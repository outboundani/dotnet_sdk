using System.Net.Http;

namespace OutboundIQ;

/// <summary>
/// One API call, with its body already serialized.
/// </summary>
/// <remarks>
/// The body is serialized once, up front, and held as bytes so a retry can build a fresh
/// <see cref="HttpRequestMessage"/> without re-serializing. An <see cref="HttpContent"/> cannot be
/// sent twice, and <see cref="HttpClient"/> refuses to resend a message it has already sent.
/// </remarks>
internal sealed class ApiRequest
{
    private ApiRequest(HttpMethod method, string path, IReadOnlyList<KeyValuePair<string, string?>>? query, byte[]? body)
    {
        Method = method;
        Path = path;
        Query = query;
        Body = body;
    }

    public HttpMethod Method { get; }

    public string Path { get; }

    public IReadOnlyList<KeyValuePair<string, string?>>? Query { get; }

    public byte[]? Body { get; }

    public static ApiRequest Get(string path, IReadOnlyList<KeyValuePair<string, string?>>? query = null) =>
        new(HttpMethod.Get, path, query, body: null);

    public static ApiRequest Post(string path, byte[] body) =>
        new(HttpMethod.Post, path, query: null, body);

    public static ApiRequest Put(string path, byte[] body) =>
        new(HttpMethod.Put, path, query: null, body);

    /// <summary>
    /// A DELETE that carries a JSON body. The Custom Dialer endpoints identify the resource to
    /// delete in the body rather than the query string.
    /// </summary>
    public static ApiRequest Delete(string path, byte[] body) =>
        new(HttpMethod.Delete, path, query: null, body);

    public override string ToString() => $"{Method} {Path}";
}
