using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Repositories;

namespace WeatherApp.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherReadingRepository _weatherRepository;

    public WeatherService(IWeatherReadingRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    public async Task<Result<WeatherReadingDto>> GetCurrentAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = await _weatherRepository.GetLatestForLocationAsync(locationId, cancellationToken);
            if (reading is null)
            {
                return Result<WeatherReadingDto>.Fail("Sem leitura atual disponível para a localização.");
            }

            var dto = new WeatherReadingDto(
                reading.Id,
                reading.ObservedAt,
                reading.Temperature,
                reading.TemperatureUnit,
                reading.HumidityPercent,
                reading.WindSpeedKmh,
                reading.Condition,
                reading.PrecipitationProbability.ToPercent(),
                reading.Location.Id
            );

            return Result<WeatherReadingDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<WeatherReadingDto>.Fail($"Erro ao obter previsão atual: {ex.Message}");
        }
    }
}
