using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.DTOs;

public record WeatherReadingDto(
    Guid Id,
    DateTimeOffset ObservedAt,
    double Temperature,
    TemperatureUnit TemperatureUnit,
    double HumidityPercent,
    double WindSpeedKmh,
    WeatherCondition Condition,
    double PrecipitationProbabilityPercent,
    Guid LocationId
);
