public record NwsPeriod(
    string Name,
    DateTimeOffset StartTime,
    bool IsDaytime,
    int? Temperature,
    string? TemperatureUnit,
    string? WindSpeed,
    string? WindDirection,
    string? ShortForecast,
    string? DetailedForecast,
    string? Icon
);

public sealed class WeatherResult
{
    public string? City { get; set; }
    public string? State { get; set; }
    public IReadOnlyList<NwsPeriod> Periods { get; set; } = Array.Empty<NwsPeriod>();
}
