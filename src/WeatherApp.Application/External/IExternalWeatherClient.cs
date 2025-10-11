using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.External;

public interface IExternalWeatherClient
{
    Task<ExternalCurrentWeather?> GetCurrentAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}

public sealed record ExternalCurrentWeather(
    double TemperatureCelsius,
    double HumidityPercent,
    double WindSpeedKmh,
    WeatherCondition Condition,
    double PrecipitationProbabilityPercent
);
