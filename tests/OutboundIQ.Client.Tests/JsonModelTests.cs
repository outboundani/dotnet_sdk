using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ.Tests;

/// <summary>
/// How the models map to the wire format.
/// </summary>
public class JsonModelTests
{
    /// <summary>
    /// Query-parameter types travel in the URL, never as JSON, so they carry no attributes.
    /// </summary>
    private static readonly HashSet<Type> NotSerialized = [typeof(NrmListAnisParams)];

    [Fact]
    public void Every_model_property_declares_its_wire_name()
    {
        // A global naming policy cannot work here — the API is snake_case on some endpoints and
        // camelCase on others — so every property must name itself. This guard catches the most
        // likely future contributor mistake: adding a property and forgetting the attribute.
        var offenders = new List<string>();

        foreach (var type in typeof(OutboundIQClient).Assembly.GetTypes())
        {
            if (!type.IsPublic || NotSerialized.Contains(type))
            {
                continue;
            }

            // Records expose a compiler-generated EqualityContract; that identifies our DTOs.
            if (type.GetProperty("EqualityContract", BindingFlags.Instance | BindingFlags.NonPublic) is null)
            {
                continue;
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetCustomAttribute<JsonPropertyNameAttribute>() is not null
                    || property.GetCustomAttribute<JsonExtensionDataAttribute>() is not null
                    || property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
                {
                    continue;
                }

                offenders.Add($"{type.Name}.{property.Name}");
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void Optional_properties_are_omitted_rather_than_sent_as_null()
    {
        var json = Serialize(new AssignmentRequest { ProspectPhone = "5559876543" });

        Assert.Equal("""{"prospect_phone":"5559876543"}""", json);
    }

    [Fact]
    public void An_unset_optional_bool_is_omitted_rather_than_sent_as_false()
    {
        // bool? rather than bool, so "not specified" and "explicitly false" stay distinguishable.
        var json = Serialize(new AssignmentRequest { ProspectPhone = "5559876543", RealTime = false });

        Assert.Contains(""""real_time":false"""", json, StringComparison.Ordinal);
        Assert.DoesNotContain("e164", json, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("3", true)]
    [InlineData("\"3\"", false)]
    public void NumberOrString_round_trips_the_form_it_arrived_in(string wireValue, bool expectedNumber)
    {
        var json = $$"""{"id":{{wireValue}},"phone":"5551234567"}""";

        var ani = JsonSerializer.Deserialize(json, TestJsonContext.Default.NrmAni)!;

        Assert.Equal(expectedNumber, ani.Id.IsNumber);
        Assert.True(ani.Id.TryGetInt64(out var id));
        Assert.Equal(3, id);

        // Re-serializing must reproduce the original token type, not coerce it.
        var round = JsonSerializer.Serialize(ani, TestJsonContext.Default.NrmAni);
        Assert.Contains($"\"id\":{wireValue}", round, StringComparison.Ordinal);
    }

    [Fact]
    public void NumberOrString_accepts_implicit_conversions()
    {
        Assert.True(((NumberOrString)3).IsNumber);
        Assert.False(((NumberOrString)"3").IsNumber);
        Assert.Equal("3", ((NumberOrString)"3").Text);
        Assert.Equal(3d, ((NumberOrString)3).Number);
    }

    [Fact]
    public void NumberOrString_preserves_integers_too_large_for_a_double()
    {
        const string big = "9007199254740993";
        var ani = JsonSerializer.Deserialize($$"""{"id":{{big}}}""", TestJsonContext.Default.NrmAni)!;

        Assert.True(ani.Id.TryGetInt64(out var id));
        Assert.Equal(9007199254740993L, id);
        Assert.Contains(big, JsonSerializer.Serialize(ani, TestJsonContext.Default.NrmAni), StringComparison.Ordinal);
    }

    [Fact]
    public void Unknown_response_properties_are_preserved()
    {
        const string json = """{"success":true,"message":"ok","futureField":42,"nested":{"a":1}}""";

        var response = JsonSerializer.Deserialize(json, TestJsonContext.Default.CustomApiResponse)!;

        Assert.True(response.Success);
        Assert.Equal("ok", response.Message);
        Assert.NotNull(response.AdditionalProperties);
        Assert.Equal(42, response.AdditionalProperties!["futureField"].GetInt32());
        Assert.Equal(1, response.AdditionalProperties["nested"].GetProperty("a").GetInt32());
    }

    [Fact]
    public void Nrm_action_response_models_a_semantic_failure_inside_a_200()
    {
        // The remediate endpoint answers HTTP 200 with this body when the ANI is inside its
        // 30-day cooldown. It is a refusal, not a success.
        var response = JsonSerializer.Deserialize(
            """{"status":"within cooldown"}""", TestJsonContext.Default.NrmActionResponse)!;

        Assert.Equal("within cooldown", response.Status);
        Assert.Null(response.Success);
        Assert.Null(response.Message);
    }

    [Fact]
    public void Assignment_response_narrows_ani_when_successful()
    {
        var success = JsonSerializer.Deserialize(
            """{"success":true,"ani":"2345678901","message":"ok"}""", TestJsonContext.Default.AssignmentResponse)!;

        Assert.True(success.Success);
        Assert.Equal("2345678901", success.Ani);

        var failure = JsonSerializer.Deserialize(
            """{"success":false,"message":"no ani available"}""", TestJsonContext.Default.AssignmentResponse)!;

        Assert.False(failure.Success);
        Assert.Null(failure.Ani);
        Assert.Equal("no ani available", failure.Message);
    }

    [Fact]
    public void Nrm_list_response_mixes_snake_case_envelope_with_camel_case_rows()
    {
        const string json = """
            {"result":"success","count":1,"total_anis":42,"can_next_page":true,
             "can_prev_page":false,"total_pages":3,
             "data":[{"id":7,"phone":"5551234567","statusLabel":"Active","last30DaysDials":120}]}
            """;

        var response = JsonSerializer.Deserialize(json, TestJsonContext.Default.NrmListAnisResponse)!;

        Assert.Equal(42, response.TotalAnis);
        Assert.True(response.CanNextPage);
        Assert.Equal(3, response.TotalPages);

        var ani = Assert.Single(response.Data);
        Assert.Equal("Active", ani.StatusLabel);
        Assert.Equal(120, ani.Last30DaysDials);
    }

    private static string Serialize(AssignmentRequest request) =>
        JsonSerializer.Serialize(request, TestJsonContext.Default.AssignmentRequest);
}

/// <summary>
/// A test-local serializer context. The SDK's own context is internal, so tests that want to
/// exercise a model directly need their own metadata for it.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified)]
[JsonSerializable(typeof(AssignmentRequest))]
[JsonSerializable(typeof(AssignmentResponse))]
[JsonSerializable(typeof(NrmAni))]
[JsonSerializable(typeof(NrmListAnisResponse))]
[JsonSerializable(typeof(NrmActionResponse))]
[JsonSerializable(typeof(CustomApiResponse))]
internal sealed partial class TestJsonContext : JsonSerializerContext;
