using WeatherApp.Domain.Enums;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.Entities;

public class WeatherReading
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTimeOffset ObservedAt { get; private set; }
    public double Temperature { get; private set; }
    public TemperatureUnit TemperatureUnit { get; private set; }
    public double HumidityPercent { get; private set; }
    public double WindSpeedKmh { get; private set; }
    public WeatherCondition Condition { get; private set; }
    public Probability PrecipitationProbability { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; }

    public WeatherReading(
        DateTimeOffset observedAt,
        double temperature,
        TemperatureUnit temperatureUnit,
        double humidityPercent,
        double windSpeedKmh,
        WeatherCondition condition,
        Probability precipitationProbability,
        Location location)
    {
        if (humidityPercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(humidityPercent), "Humidity must be 0-100%.");
        if (windSpeedKmh < 0)
            throw new ArgumentOutOfRangeException(nameof(windSpeedKmh), "Wind speed cannot be negative.");

        ObservedAt = observedAt;
        Temperature = temperature;
        TemperatureUnit = temperatureUnit;
        HumidityPercent = humidityPercent;
        WindSpeedKmh = windSpeedKmh;
        Condition = condition;
        PrecipitationProbability = precipitationProbability ?? throw new ArgumentNullException(nameof(precipitationProbability));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        LocationId = location.Id;
    }

    private WeatherReading()
    {
        Condition = WeatherCondition.Clear;
        PrecipitationProbability = Probability.FromPercent(0);
        Location = null!;
        LocationId = Guid.Empty;
    }

    public void Update(
        DateTimeOffset observedAt,
        double temperature,
        TemperatureUnit temperatureUnit,
        double humidityPercent,
        double windSpeedKmh,
        WeatherCondition condition,
        Probability precipitationProbability,
        Location location)
    {
        if (humidityPercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(humidityPercent), "Humidity must be 0-100%.");
        if (windSpeedKmh < 0)
            throw new ArgumentOutOfRangeException(nameof(windSpeedKmh), "Wind speed cannot be negative.");

        ObservedAt = observedAt;
        Temperature = temperature;
        TemperatureUnit = temperatureUnit;
        HumidityPercent = humidityPercent;
        WindSpeedKmh = windSpeedKmh;
        Condition = condition;
        PrecipitationProbability = precipitationProbability ?? throw new ArgumentNullException(nameof(precipitationProbability));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        LocationId = location.Id;
    }
}
