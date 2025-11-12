using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class WeatherService(IHttpClientFactory http)
{
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<WeatherResult> GetSevenDayAsync(double lat, double lon, CancellationToken ct = default)
    {
        var client = http.CreateClient("nws");

        string latS = lat.ToString(CultureInfo.InvariantCulture);
        string lonS = lon.ToString(CultureInfo.InvariantCulture);

        using var pointsResp = await client.GetAsync($"points/{latS},{lonS}", ct);
        pointsResp.EnsureSuccessStatusCode();

        using var pointsDoc = JsonDocument.Parse(await pointsResp.Content.ReadAsStreamAsync(ct));
        var props = pointsDoc.RootElement.GetProperty("properties");

        var forecastUrl = props.GetProperty("forecast").GetString();
        var city = props.GetProperty("relativeLocation").GetProperty("properties").GetProperty("city").GetString();
        var state = props.GetProperty("relativeLocation").GetProperty("properties").GetProperty("state").GetString();

        if (string.IsNullOrWhiteSpace(forecastUrl))
            throw new InvalidOperationException("NWS did not return a forecast URL for these coordinates.");

        using var fcResp = await client.GetAsync(forecastUrl, ct);
        fcResp.EnsureSuccessStatusCode();

        using var fcDoc = JsonDocument.Parse(await fcResp.Content.ReadAsStreamAsync(ct));
        var periods = fcDoc.RootElement.GetProperty("properties").GetProperty("periods");

        var list = new List<NwsPeriod>(periods.GetArrayLength());
        foreach (var p in periods.EnumerateArray())
        {
            list.Add(new NwsPeriod(
                Name: p.GetProperty("name").GetString() ?? "",
                StartTime: p.GetProperty("startTime").GetDateTimeOffset(),
                IsDaytime: p.GetProperty("isDaytime").GetBoolean(),
                Temperature: p.TryGetProperty("temperature", out var tEl) && tEl.ValueKind != JsonValueKind.Null ? tEl.GetInt32() : (int?)null,
                TemperatureUnit: p.GetProperty("temperatureUnit").GetString(),
                WindSpeed: p.GetProperty("windSpeed").GetString(),
                WindDirection: p.GetProperty("windDirection").GetString(),
                ShortForecast: p.GetProperty("shortForecast").GetString(),
                DetailedForecast: p.GetProperty("detailedForecast").GetString(),
                Icon: p.TryGetProperty("icon", out var iconEl) ? iconEl.GetString() : null
            ));
        }

        return new WeatherResult { City = city, State = state, Periods = list };
    }
}
