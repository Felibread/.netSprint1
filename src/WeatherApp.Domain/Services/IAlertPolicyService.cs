using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Domain.Services;

public interface IAlertPolicyService
{
    Alert? TryCreateRainAlert(WeatherReading reading, double rainProbabilityPercentThreshold);
    Alert? TryCreateExtremeTemperatureAlert(WeatherReading reading, double minCelsius, double maxCelsius);
}
