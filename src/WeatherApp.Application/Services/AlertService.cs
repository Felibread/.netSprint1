using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Repositories;
using WeatherApp.Domain.Services;

namespace WeatherApp.Application.Services;

public class AlertService : IAlertService
{
    private readonly IWeatherReadingRepository _weatherRepository;
    private readonly IAlertPolicyService _alertPolicy;

    public AlertService(IWeatherReadingRepository weatherRepository, IAlertPolicyService alertPolicy)
    {
        _weatherRepository = weatherRepository;
        _alertPolicy = alertPolicy;
    }

    public async Task<Result<IReadOnlyList<AlertDto>>> EvaluateAlertsAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = await _weatherRepository.GetLatestForLocationAsync(locationId, cancellationToken);
            if (reading is null)
            {
                return Result<IReadOnlyList<AlertDto>>.Fail("Sem leitura para avaliar alertas.");
            }

            var alerts = new List<AlertDto>();

            var rainAlert = _alertPolicy.TryCreateRainAlert(reading, 60);
            if (rainAlert is not null)
            {
                alerts.Add(new AlertDto(rainAlert.Id, rainAlert.Type, rainAlert.Title, rainAlert.Message, rainAlert.CreatedAt, rainAlert.ExpiresAt));
            }

            var extreme = _alertPolicy.TryCreateExtremeTemperatureAlert(reading, 5, 35);
            if (extreme is not null)
            {
                alerts.Add(new AlertDto(extreme.Id, extreme.Type, extreme.Title, extreme.Message, extreme.CreatedAt, extreme.ExpiresAt));
            }

            return Result<IReadOnlyList<AlertDto>>.Ok(alerts);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<AlertDto>>.Fail($"Erro ao avaliar alertas: {ex.Message}");
        }
    }
}
