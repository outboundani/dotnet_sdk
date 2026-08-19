namespace OutboundIQ.Tests;

/// <summary>
/// The executable specification for every endpoint: verb, URL, and request body.
/// </summary>
/// <remarks>
/// Ported from the TypeScript SDK's <c>test/resources.test.ts</c>, which drives the same
/// assertions from the same table. Keeping the two in step is what keeps the SDKs interchangeable.
/// </remarks>
public class EndpointSpecTests
{
    private const string Base = TestClient.BaseUrl;

    private static readonly Dictionary<string, EndpointCase> Cases = new(StringComparer.Ordinal)
    {
        ["assignment.next"] = new(
            "POST",
            $"{Base}/assignment",
            client => client.Assignment.NextAsync(new AssignmentRequest
            {
                ProspectPhone = "5559876543",
                ProspectZip = "90210",
            }),
            """{"prospect_phone":"5559876543","prospect_zip":"90210"}"""),

        ["assignment.batch"] = new(
            "POST",
            $"{Base}/assignment/batch",
            client => client.Assignment.BatchAsync(new AssignmentBatchRequest
            {
                Leads =
                [
                    new AssignmentBatchLead { RowId = "lead-1", ProspectPhone = "5559876543", ProspectZip = "90210" },
                    new AssignmentBatchLead { RowId = "lead-2", ProspectPhone = "5551112222" },
                ],
                E164 = true,
            }),
            """
            {"leads":[{"row_id":"lead-1","prospect_phone":"5559876543","prospect_zip":"90210"},
            {"row_id":"lead-2","prospect_phone":"5551112222"}],"e164":true}
            """),

        ["dials.create"] = new(
            "POST",
            $"{Base}/dials",
            client => client.Dials.CreateAsync(SampleDial),
            """
            {"campaign_id":"abc-123","campaign_name":"Q2 Outbound Push","agent_name":"Jane Doe",
            "from_number":"5551234567","to_number":"5559876543","disposition_name":"Sale",
            "datetime":"2026-04-10 14:32:15","call_direction":"Outbound","zip":"90210",
            "sys_created_date_original":"2026-04-01","total_dial_attempts":3,
            "skill_name":"Sales Tier 1","lead_source":"facebook-ads",
            "dial_id":"550e8400-e29b-41d4-a716-446655440000"}
            """),

        ["custom.campaigns.create"] = new(
            "POST",
            $"{Base}/custom/campaigns",
            client => client.Custom.Campaigns.CreateAsync(new CustomCampaign
            {
                Id = "c-1",
                Name = "My Campaign",
                Type = CustomCampaignTypes.Outbound,
            }),
            """{"id":"c-1","name":"My Campaign","type":"Outbound"}"""),

        ["custom.campaigns.update"] = new(
            "PUT",
            $"{Base}/custom/campaigns",
            client => client.Custom.Campaigns.UpdateAsync(new CustomCampaignUpdate { Id = "c-1", Name = "Renamed" }),
            """{"id":"c-1","name":"Renamed"}"""),

        ["custom.campaigns.get"] = new(
            "GET",
            $"{Base}/custom/campaigns?id=c-1",
            client => client.Custom.Campaigns.GetAsync("c-1")),

        ["custom.campaigns.delete"] = new(
            "DELETE",
            $"{Base}/custom/campaigns",
            client => client.Custom.Campaigns.DeleteAsync("c-1"),
            """{"id":"c-1"}"""),

        ["custom.dispos.create"] = new(
            "POST",
            $"{Base}/custom/dispos",
            client => client.Custom.Dispos.CreateAsync(new CustomDispo
            {
                Id = "d-1",
                Name = "Sale",
                Type = CustomDispoTypes.Agent,
                Contact = true,
                Success = true,
            }),
            """{"id":"d-1","name":"Sale","type":"Agent","contact":true,"success":true}"""),

        ["custom.dispos.update"] = new(
            "PUT",
            $"{Base}/custom/dispos",
            client => client.Custom.Dispos.UpdateAsync(new CustomDispoUpdate { Id = "d-1", Contact = false }),
            """{"id":"d-1","contact":false}"""),

        ["custom.dispos.get"] = new(
            "GET",
            $"{Base}/custom/dispos?id=d-1",
            client => client.Custom.Dispos.GetAsync("d-1")),

        ["custom.dispos.delete"] = new(
            "DELETE",
            $"{Base}/custom/dispos",
            client => client.Custom.Dispos.DeleteAsync("d-1"),
            """{"id":"d-1"}"""),

        ["custom.anis.create"] = new(
            "POST",
            $"{Base}/custom/anis",
            client => client.Custom.Anis.CreateAsync(new CustomAni
            {
                CountryCode = CountryCodes.Us,
                Number = "5551234567",
                InboundGroupId = "g-1",
            }),
            """{"country_code":"+1","number":"5551234567","inbound_group_id":"g-1"}"""),

        ["custom.anis.update"] = new(
            "PUT",
            $"{Base}/custom/anis",
            client => client.Custom.Anis.UpdateAsync(new CustomAniUpdate { Number = "5551234567", IsBranded = true }),
            """{"number":"5551234567","is_branded":true}"""),

        ["custom.anis.get"] = new(
            "GET",
            $"{Base}/custom/anis?number=5551234567",
            client => client.Custom.Anis.GetAsync("5551234567"),
            ResponseJson: """{"success":true,"ani":{"number":"5551234567"}}"""),

        ["custom.anis.delete"] = new(
            "DELETE",
            $"{Base}/custom/anis",
            client => client.Custom.Anis.DeleteAsync("5551234567"),
            """{"number":"5551234567"}"""),

        ["nrm.listAnis"] = new(
            "GET",
            $"{Base}/nrm/anis?page=2&page_size=50",
            client => client.Nrm.ListAnisAsync(new NrmListAnisParams { Page = 2, PageSize = 50 })),

        ["nrm.listAnis.noParams"] = new(
            "GET",
            $"{Base}/nrm/anis",
            client => client.Nrm.ListAnisAsync()),

        ["nrm.remediate"] = new(
            "POST",
            $"{Base}/nrm/remediate",
            client => client.Nrm.RemediateAsync(new NrmRemediateRequest { Ani = "5551234567", Carrier = "123456" }),
            """{"ani":"5551234567","carrier":"123456"}"""),

        ["nrm.pause"] = new(
            "POST",
            $"{Base}/nrm/pause",
            client => client.Nrm.PauseAsync(new NrmPauseRequest { Ani = "5551234567", Note = "cooling off" }),
            """{"ani":"5551234567","note":"cooling off"}"""),

        ["nrm.activate"] = new(
            "POST",
            $"{Base}/nrm/activate",
            client => client.Nrm.ActivateAsync(new NrmActivateRequest { Ani = "5551234567", Date = "2026-05-01" }),
            """{"ani":"5551234567","date":"2026-05-01"}"""),

        ["aniPlanner.generate"] = new(
            "POST",
            $"{Base}/ani-planner/generate",
            client => client.AniPlanner.GenerateAsync(new AniPlannerGenerateRequest
            {
                DateStart = "2026-04-01",
                DailyDialsTarget = AniPlannerDailyDialsTargets.Best,
            }),
            """{"dateStart":"2026-04-01","dailyDialsTarget":"BEST"}"""),

        // An empty request must still send a body, and that body must be exactly {}.
        ["aniPlanner.generate.empty"] = new(
            "POST",
            $"{Base}/ani-planner/generate",
            client => client.AniPlanner.GenerateAsync(),
            "{}"),

        ["liveFeed.ringcx.upload"] = new(
            "POST",
            $"{Base}/live-feed/ringcx",
            client => client.LiveFeed.RingCx.UploadAsync(new RingCxUploadRequest
            {
                CampaignId = "camp-1",
                Lead = new RingCxLead { LeadPhone = "5559876543", FirstName = "Jane" },
                Options = new RingCxUploadOptions { DialPriority = RingCxDialPriorities.Immediate },
            }),
            """
            {"campaignId":"camp-1","lead":{"leadPhone":"5559876543","firstName":"Jane"},
            "options":{"dialPriority":"IMMEDIATE"}}
            """),
    };

    private static DialRecord SampleDial => new()
    {
        CampaignId = "abc-123",
        CampaignName = "Q2 Outbound Push",
        AgentName = "Jane Doe",
        FromNumber = "5551234567",
        ToNumber = "5559876543",
        DispositionName = "Sale",
        DateTime = "2026-04-10 14:32:15",
        CallDirection = CallDirections.Outbound,
        Zip = "90210",
        SysCreatedDateOriginal = "2026-04-01",
        TotalDialAttempts = 3,
        SkillName = "Sales Tier 1",
        LeadSource = "facebook-ads",
        DialId = "550e8400-e29b-41d4-a716-446655440000",
    };

    // Keyed by name rather than passing the case itself: EndpointCase holds a delegate and so is
    // not serializable, which would force xUnit to disable discovery enumeration and collapse
    // every case into a single un-rerunnable test.
    public static TheoryData<string> CaseNames => [.. Cases.Keys];

    [Theory]
    [MemberData(nameof(CaseNames))]
    public async Task Endpoint_sends_the_expected_request(string caseName)
    {
        var testCase = Cases[caseName];
        var handler = new StubHttpMessageHandler(Responses.Ok(testCase.ResponseJson));
        using var client = TestClient.Create(handler);

        await testCase.Run(client);

        var call = handler.Single();
        Assert.Equal(testCase.Method, call.Method);
        Assert.Equal(testCase.Url, call.Url);

        if (testCase.ExpectedBody is not null)
        {
            JsonAssert.Equivalent(testCase.ExpectedBody, call.Body);
        }
        else
        {
            // No body means no content, and therefore no Content-Type header.
            Assert.True(string.IsNullOrEmpty(call.Body));
            Assert.False(call.HasHeader("Content-Type"));
        }
    }

    [Fact]
    public void Every_endpoint_is_covered()
    {
        // Guards against a case being dropped during a refactor. The TypeScript SDK exposes 16
        // public methods; the extra entries here are variants of the same calls.
        Assert.Equal(23, Cases.Count);
    }

    internal sealed record EndpointCase(
        string Method,
        string Url,
        Func<IOutboundIQClient, Task> Run,
        string? ExpectedBody = null,
        string ResponseJson = """{"success":true}""");
}
