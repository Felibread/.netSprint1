using WeatherApp.Domain.Enums;

namespace WeatherApp.Api.ViewModels.Weather;

public class WeatherReadingViewModel
{
    public Guid Id { get; init; }
    public DateTimeOffset ObservedAt { get; init; }
    public double Temperature { get; init; }
    public TemperatureUnit TemperatureUnit { get; init; }
    public double HumidityPercent { get; init; }
    public double WindSpeedKmh { get; init; }
    public WeatherCondition Condition { get; init; }
    public double PrecipitationProbabilityPercent { get; init; }
}

