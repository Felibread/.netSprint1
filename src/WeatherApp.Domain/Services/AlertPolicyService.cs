using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Domain.Services;

public class AlertPolicyService : IAlertPolicyService
{
    public Alert? TryCreateRainAlert(WeatherReading reading, double rainProbabilityPercentThreshold)
    {
        if (reading.PrecipitationProbability.ToPercent() >= rainProbabilityPercentThreshold)
        {
            var title = "Possível chuva";
            var message = $"Probabilidade de {reading.PrecipitationProbability.ToPercent():0}% de precipitação.";
            return new Alert(AlertType.Rain, title, message, DateTimeOffset.UtcNow.AddHours(6));
        }
        return null;
    }

    public Alert? TryCreateExtremeTemperatureAlert(WeatherReading reading, double minCelsius, double maxCelsius)
    {
        double tempCelsius = reading.TemperatureUnit == TemperatureUnit.Celsius
            ? reading.Temperature
            : (reading.Temperature - 32.0) * 5.0 / 9.0;

        if (tempCelsius <= minCelsius)
        {
            var title = "Frio extremo";
            var message = $"Temperatura prevista {tempCelsius:0.#}°C abaixo do limite {minCelsius:0.#}°C.";
            return new Alert(AlertType.ExtremeTemperature, title, message, DateTimeOffset.UtcNow.AddHours(6));
        }

        if (tempCelsius >= maxCelsius)
        {
            var title = "Calor extremo";
            var message = $"Temperatura prevista {tempCelsius:0.#}°C acima do limite {maxCelsius:0.#}°C.";
            return new Alert(AlertType.ExtremeTemperature, title, message, DateTimeOffset.UtcNow.AddHours(6));
        }

        return null;
    }
}
