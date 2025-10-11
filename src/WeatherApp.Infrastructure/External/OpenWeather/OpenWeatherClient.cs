using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using WeatherApp.Application.External;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Infrastructure.External.OpenWeather;

public sealed class OpenWeatherClient : IExternalWeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeather:ApiKey"] ?? string.Empty;
    }

    public async Task<ExternalCurrentWeather?> GetCurrentAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            return null;
        }

        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await response.Content.ReadFromJsonAsync<OpenWeatherCurrentResponse>(cancellationToken: cancellationToken);
        if (data is null)
        {
            return null;
        }

        var condition = MapCondition(data.weather?.FirstOrDefault()?.main);
        var windKmh = (data.wind?.speed ?? 0) * 3.6; // m/s -> km/h
        var humidity = data.main?.humidity ?? 0;
        var tempC = data.main?.temp ?? 0;

        // OpenWeather current endpoint does not provide probability of precipitation directly.
        // We set to 0 and let forecasts fill it in a future enhancement.
        return new ExternalCurrentWeather(
            TemperatureCelsius: tempC,
            HumidityPercent: humidity,
            WindSpeedKmh: windKmh,
            Condition: condition,
            PrecipitationProbabilityPercent: 0
        );
    }

    private static WeatherCondition MapCondition(string? main)
    {
        return main?.ToLowerInvariant() switch
        {
            "clear" => WeatherCondition.Clear,
            "clouds" => WeatherCondition.Clouds,
            "rain" => WeatherCondition.Rain,
            "thunderstorm" => WeatherCondition.Thunderstorm,
            "drizzle" => WeatherCondition.Drizzle,
            "snow" => WeatherCondition.Snow,
            "mist" => WeatherCondition.Mist,
            "fog" => WeatherCondition.Fog,
            "haze" => WeatherCondition.Haze,
            "smoke" => WeatherCondition.Smoke,
            "dust" => WeatherCondition.Dust,
            "sand" => WeatherCondition.Sand,
            "ash" => WeatherCondition.Ash,
            "squall" => WeatherCondition.Squall,
            "tornado" => WeatherCondition.Tornado,
            _ => WeatherCondition.Unknown
        };
    }
}

// Minimal subset of the OpenWeather /weather response
internal sealed class OpenWeatherCurrentResponse
{
    public List<WeatherInfo>? weather { get; set; }
    public MainInfo? main { get; set; }
    public WindInfo? wind { get; set; }

    internal sealed class WeatherInfo { public string? main { get; set; } public string? description { get; set; } }
    internal sealed class MainInfo { public double temp { get; set; } public double humidity { get; set; } }
    internal sealed class WindInfo { public double speed { get; set; } }
}
