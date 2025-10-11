using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;
using WeatherApp.Domain.Repositories;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherReadingRepository _weatherRepository;
    private readonly IExternalWeatherClient _externalClient;
    private readonly ILocationRepository _locationRepository;

    public WeatherService(IWeatherReadingRepository weatherRepository, IExternalWeatherClient externalClient, ILocationRepository locationRepository)
    {
        _weatherRepository = weatherRepository;
        _externalClient = externalClient;
        _locationRepository = locationRepository;
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

    public async Task<Result<WeatherReadingDto>> RefreshCurrentAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (location is null)
                return Result<WeatherReadingDto>.Fail("Localização não encontrada.");

            var ext = await _externalClient.GetCurrentAsync(location.Coordinates.Latitude, location.Coordinates.Longitude, cancellationToken);
            if (ext is null)
                return Result<WeatherReadingDto>.Fail("Falha ao consultar provedor externo.");

            var reading = new WeatherReading(
                observedAt: DateTimeOffset.UtcNow,
                temperature: ext.TemperatureCelsius,
                temperatureUnit: TemperatureUnit.Celsius,
                humidityPercent: ext.HumidityPercent,
                windSpeedKmh: ext.WindSpeedKmh,
                condition: ext.Condition,
                precipitationProbability: Probability.FromPercent(ext.PrecipitationProbabilityPercent),
                location: location
            );

            await _weatherRepository.AddAsync(reading, cancellationToken);

            var dto = new WeatherReadingDto(
                reading.Id,
                reading.ObservedAt,
                reading.Temperature,
                reading.TemperatureUnit,
                reading.HumidityPercent,
                reading.WindSpeedKmh,
                reading.Condition,
                reading.PrecipitationProbability.ToPercent(),
                location.Id
            );
            return Result<WeatherReadingDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<WeatherReadingDto>.Fail($"Erro ao atualizar leitura: {ex.Message}");
        }
    }
}
