namespace OutboundIQ;

/// <summary>
/// Custom dialer integration: sync campaigns, dispositions, and ANIs from any dialer into
/// outboundIQ. See <see href="https://docs.outboundiq.cloud/reference/custom/"/>.
/// </summary>
public interface ICustomDialerResource
{
    /// <summary>Campaign synchronization.</summary>
    ICustomCampaigns Campaigns { get; }

    /// <summary>Disposition synchronization.</summary>
    ICustomDispos Dispos { get; }

    /// <summary>ANI synchronization.</summary>
    ICustomAnis Anis { get; }
}

/// <summary>
/// Campaign synchronization, on <c>/custom/campaigns</c>.
/// </summary>
public interface ICustomCampaigns
{
    /// <summary>Creates a campaign. <c>POST /custom/campaigns</c></summary>
    /// <param name="campaign">The campaign to create.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> CreateAsync(CustomCampaign campaign, CancellationToken cancellationToken = default);

    /// <summary>Updates a campaign. <c>PUT /custom/campaigns</c></summary>
    /// <param name="campaign">The changes to apply.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> UpdateAsync(CustomCampaignUpdate campaign, CancellationToken cancellationToken = default);

    /// <summary>Fetches a campaign. <c>GET /custom/campaigns?id=</c></summary>
    /// <param name="id">Your dialer's identifier for the campaign.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Deletes a campaign. <c>DELETE /custom/campaigns</c></summary>
    /// <param name="id">Your dialer's identifier for the campaign.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <remarks>The identifier is sent in the request body, not the query string.</remarks>
    Task<CustomApiResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Disposition synchronization, on <c>/custom/dispos</c>.
/// </summary>
public interface ICustomDispos
{
    /// <summary>Creates a disposition. <c>POST /custom/dispos</c></summary>
    /// <param name="dispo">The disposition to create.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> CreateAsync(CustomDispo dispo, CancellationToken cancellationToken = default);

    /// <summary>Updates a disposition. <c>PUT /custom/dispos</c></summary>
    /// <param name="dispo">The changes to apply.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> UpdateAsync(CustomDispoUpdate dispo, CancellationToken cancellationToken = default);

    /// <summary>Fetches a disposition. <c>GET /custom/dispos?id=</c></summary>
    /// <param name="id">Your dialer's identifier for the disposition.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Deletes a disposition. <c>DELETE /custom/dispos</c></summary>
    /// <param name="id">Your dialer's identifier for the disposition.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <remarks>The identifier is sent in the request body, not the query string.</remarks>
    Task<CustomApiResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// ANI synchronization, on <c>/custom/anis</c>.
/// </summary>
public interface ICustomAnis
{
    /// <summary>Creates an ANI. <c>POST /custom/anis</c></summary>
    /// <param name="ani">The ANI to create.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> CreateAsync(CustomAni ani, CancellationToken cancellationToken = default);

    /// <summary>Updates an ANI. <c>PUT /custom/anis</c></summary>
    /// <param name="ani">The changes to apply.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<CustomApiResponse> UpdateAsync(CustomAniUpdate ani, CancellationToken cancellationToken = default);

    /// <summary>Fetches an ANI. <c>GET /custom/anis?number=</c></summary>
    /// <param name="number">The phone number.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<GetCustomAniResponse> GetAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>Deletes an ANI. <c>DELETE /custom/anis</c></summary>
    /// <param name="number">The phone number.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <remarks>The number is sent in the request body, not the query string.</remarks>
    Task<CustomApiResponse> DeleteAsync(string number, CancellationToken cancellationToken = default);
}
