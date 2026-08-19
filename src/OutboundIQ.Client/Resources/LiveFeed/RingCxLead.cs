using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A lead to push into a running RingCX campaign.
/// </summary>
/// <remarks>
/// The caller ID is assigned by outboundIQ. Any caller ID supplied here is overwritten.
/// </remarks>
public sealed record RingCxLead
{
    /// <summary>The prospect's phone number.</summary>
    [JsonPropertyName("leadPhone")]
    public required string LeadPhone { get; init; }

    /// <summary>The prospect's first name.</summary>
    [JsonPropertyName("firstName")]
    public string? FirstName { get; init; }

    /// <summary>The prospect's last name.</summary>
    [JsonPropertyName("lastName")]
    public string? LastName { get; init; }

    /// <summary>The prospect's email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>The prospect's ZIP code.</summary>
    [JsonPropertyName("zip")]
    public string? Zip { get; init; }

    /// <summary>The prospect's city.</summary>
    [JsonPropertyName("city")]
    public string? City { get; init; }

    /// <summary>First address line.</summary>
    [JsonPropertyName("address1")]
    public string? Address1 { get; init; }

    /// <summary>Second address line.</summary>
    [JsonPropertyName("address2")]
    public string? Address2 { get; init; }

    /// <summary>The prospect's state.</summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>Free-form auxiliary field 1.</summary>
    [JsonPropertyName("auxData1")]
    public string? AuxData1 { get; init; }

    /// <summary>Free-form auxiliary field 2.</summary>
    [JsonPropertyName("auxData2")]
    public string? AuxData2 { get; init; }

    /// <summary>Free-form auxiliary field 3.</summary>
    [JsonPropertyName("auxData3")]
    public string? AuxData3 { get; init; }

    /// <summary>Free-form auxiliary field 4.</summary>
    [JsonPropertyName("auxData4")]
    public string? AuxData4 { get; init; }

    /// <summary>Free-form auxiliary field 5.</summary>
    [JsonPropertyName("auxData5")]
    public string? AuxData5 { get; init; }
}
