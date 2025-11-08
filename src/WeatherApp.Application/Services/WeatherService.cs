using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;
using System.Linq;
using WeatherApp.Application.Abstractions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Repositories;
using WeatherApp.Domain.ValueObjects;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherReadingRepository _weatherRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WeatherService(
        IWeatherReadingRepository weatherRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork)
    {
        _weatherRepository = weatherRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
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

            return Result<WeatherReadingDto>.Ok(ToDto(reading));
        }
        catch (Exception ex)
        {
            return Result<WeatherReadingDto>.Fail($"Erro ao obter previsão atual: {ex.Message}");
        }
    }

    public async Task<Result<WeatherReadingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = await _weatherRepository.GetByIdAsync(id, cancellationToken);
            if (reading is null)
            {
                return Result<WeatherReadingDto>.Fail("Leitura meteorológica não encontrada.");
            }

            return Result<WeatherReadingDto>.Ok(ToDto(reading));
        }
        catch (Exception ex)
        {
            return Result<WeatherReadingDto>.Fail($"Erro ao obter leitura: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<WeatherReadingDto>>> SearchAsync(
        Guid? locationId,
        DateTimeOffset? observedFrom,
        DateTimeOffset? observedTo,
        WeatherCondition? condition,
        double? minTemperature,
        double? maxTemperature,
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return Result<PagedResult<WeatherReadingDto>>.Fail("Parâmetros de paginação inválidos.");
            }

            var (items, totalCount) = await _weatherRepository.SearchAsync(
                locationId,
                observedFrom,
                observedTo,
                condition,
                minTemperature,
                maxTemperature,
                pageNumber,
                pageSize,
                sortBy,
                ascending,
                cancellationToken);

            var dtos = items.Select(ToDto).ToList();
            var paged = new PagedResult<WeatherReadingDto>(dtos, pageNumber, pageSize, totalCount);
            return Result<PagedResult<WeatherReadingDto>>.Ok(paged);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<WeatherReadingDto>>.Fail($"Erro na busca de leituras: {ex.Message}");
        }
    }

    public async Task<Result<WeatherReadingDto>> CreateAsync(WeatherReadingUpsertDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _locationRepository.GetTrackedAsync(dto.LocationId, cancellationToken);
            if (location is null)
            {
                return Result<WeatherReadingDto>.Fail("Localização informada não existe.");
            }

            var reading = new WeatherReading(
                dto.ObservedAt,
                dto.Temperature,
                dto.TemperatureUnit,
                dto.HumidityPercent,
                dto.WindSpeedKmh,
                dto.Condition,
                Probability.FromPercent(dto.PrecipitationProbabilityPercent),
                location);

            await _weatherRepository.AddAsync(reading, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // EF Core não rastreia automaticamente navegação após SaveChanges quando instanciada manualmente,
            // garantir que LocationId esteja correto no DTO de retorno
            return Result<WeatherReadingDto>.Ok(ToDto(reading));
        }
        catch (Exception ex)
        {
            return Result<WeatherReadingDto>.Fail($"Erro ao criar leitura: {ex.Message}");
        }
    }

    public async Task<Result<WeatherReadingDto>> UpdateAsync(Guid id, WeatherReadingUpsertDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = await _weatherRepository.GetTrackedAsync(id, cancellationToken);
            if (reading is null)
            {
                return Result<WeatherReadingDto>.Fail("Leitura meteorológica não encontrada.");
            }

            var location = await _locationRepository.GetTrackedAsync(dto.LocationId, cancellationToken);
            if (location is null)
            {
                return Result<WeatherReadingDto>.Fail("Localização informada não existe.");
            }

            reading.Update(
                dto.ObservedAt,
                dto.Temperature,
                dto.TemperatureUnit,
                dto.HumidityPercent,
                dto.WindSpeedKmh,
                dto.Condition,
                Probability.FromPercent(dto.PrecipitationProbabilityPercent),
                location);

            await _weatherRepository.UpdateAsync(reading, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<WeatherReadingDto>.Ok(ToDto(reading));
        }
        catch (Exception ex)
        {
            return Result<WeatherReadingDto>.Fail($"Erro ao atualizar leitura: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _weatherRepository.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Erro ao remover leitura: {ex.Message}");
        }
    }

    private static WeatherReadingDto ToDto(WeatherReading reading)
    {
        return new WeatherReadingDto(
            reading.Id,
            reading.ObservedAt,
            reading.Temperature,
            reading.TemperatureUnit,
            reading.HumidityPercent,
            reading.WindSpeedKmh,
            reading.Condition,
            reading.PrecipitationProbability.ToPercent(),
            reading.LocationId != Guid.Empty ? reading.LocationId : reading.Location.Id);
    }
}
