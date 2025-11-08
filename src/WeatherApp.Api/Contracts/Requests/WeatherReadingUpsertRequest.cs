using System.ComponentModel.DataAnnotations;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Api.Contracts.Requests;

public class WeatherReadingUpsertRequest
{
    [Required]
    public DateTimeOffset ObservedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public double Temperature { get; set; }

    [Required]
    public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;

    [Range(0, 100)]
    public double HumidityPercent { get; set; }

    [Range(0, double.MaxValue)]
    public double WindSpeedKmh { get; set; }

    [Required]
    public WeatherCondition Condition { get; set; } = WeatherCondition.Clear;

    [Range(0, 100)]
    public double PrecipitationProbabilityPercent { get; set; }

    [Required]
    public Guid LocationId { get; set; }
}

