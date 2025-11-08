using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.DTOs;

public record WeatherReadingUpsertDto(
    DateTimeOffset ObservedAt,
    double Temperature,
    TemperatureUnit TemperatureUnit,
    double HumidityPercent,
    double WindSpeedKmh,
    WeatherCondition Condition,
    double PrecipitationProbabilityPercent,
    Guid LocationId);

