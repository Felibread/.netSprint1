using System.Linq;
using WeatherApp.Application.Abstractions;
using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Repositories;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LocationService(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LocationDto>> CreateAsync(string name, double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = new Location(name, new Coordinates(latitude, longitude));
            await _locationRepository.AddAsync(location, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new LocationDto(location.Id, location.Name, location.Coordinates.Latitude, location.Coordinates.Longitude);
            return Result<LocationDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<LocationDto>.Fail($"Erro ao criar local: {ex.Message}");
        }
    }

    public async Task<Result<LocationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _locationRepository.GetByIdAsync(id, cancellationToken);
            if (location is null)
            {
                return Result<LocationDto>.Fail("Localização não encontrada.");
            }

            var dto = new LocationDto(location.Id, location.Name, location.Coordinates.Latitude, location.Coordinates.Longitude);
            return Result<LocationDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<LocationDto>.Fail($"Erro ao obter localização: {ex.Message}");
        }
    }

    public async Task<Result<LocationDto>> UpdateAsync(Guid id, string name, double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _locationRepository.GetTrackedAsync(id, cancellationToken);
            if (location is null)
            {
                return Result<LocationDto>.Fail("Localização não encontrada.");
            }

            location.Update(name, new Coordinates(latitude, longitude));
            await _locationRepository.UpdateAsync(location, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new LocationDto(location.Id, location.Name, location.Coordinates.Latitude, location.Coordinates.Longitude);
            return Result<LocationDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<LocationDto>.Fail($"Erro ao atualizar localização: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _locationRepository.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Erro ao remover localização: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<LocationDto>>> SearchAsync(
        string? name,
        double? minLatitude,
        double? maxLatitude,
        double? minLongitude,
        double? maxLongitude,
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
                return Result<PagedResult<LocationDto>>.Fail("Parâmetros de paginação inválidos.");
            }

            var (items, totalCount) = await _locationRepository.SearchAsync(
                name,
                minLatitude,
                maxLatitude,
                minLongitude,
                maxLongitude,
                pageNumber,
                pageSize,
                sortBy,
                ascending,
                cancellationToken);

            var dtos = items
                .Select(l => new LocationDto(l.Id, l.Name, l.Coordinates.Latitude, l.Coordinates.Longitude))
                .ToList();

            var paged = new PagedResult<LocationDto>(dtos, pageNumber, pageSize, totalCount);
            return Result<PagedResult<LocationDto>>.Ok(paged);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<LocationDto>>.Fail($"Erro na busca de localizações: {ex.Message}");
        }
    }
}
