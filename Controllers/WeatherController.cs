using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("weather")]
public class WeatherController(WeatherService svc) : ControllerBase
{
    [HttpGet("forecast")]
    public async Task<IActionResult> Forecast([FromQuery] double lat, [FromQuery] double lng, CancellationToken ct)
    {
        try
        {
            var result = await svc.GetSevenDayAsync(lat, lng, ct);
            return Ok(result);
        }
        catch
        {
            return Problem("Failed to load forecast from NWS.");
        }
    }
}
