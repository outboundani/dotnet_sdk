namespace OutboundIQ;

/// <summary>
/// Query parameters for <c>GET /nrm/anis</c>.
/// </summary>
/// <remarks>
/// These travel in the query string rather than a JSON body, so they carry no
/// <c>JsonPropertyName</c> attributes.
/// </remarks>
public sealed record NrmListAnisParams
{
    /// <summary>One-based page number. The API defaults to 1.</summary>
    public int? Page { get; init; }

    /// <summary>Page size, maximum 1000. The API defaults to 1000.</summary>
    public int? PageSize { get; init; }

    /// <summary>Filter to numbers starting with this prefix.</summary>
    public string? Number { get; init; }
}
