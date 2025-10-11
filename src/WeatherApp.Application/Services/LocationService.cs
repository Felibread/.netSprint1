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

    public async Task<Result<IReadOnlyList<LocationDto>>> SearchAsync(string query, int limit = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return Result<IReadOnlyList<LocationDto>>.Fail("Consulta vazia.");

            var locations = await _locationRepository.SearchByNameAsync(query.Trim(), limit, cancellationToken);
            var dtos = locations.Select(l => new LocationDto(l.Id, l.Name, l.Coordinates.Latitude, l.Coordinates.Longitude)).ToList();
            return Result<IReadOnlyList<LocationDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<LocationDto>>.Fail($"Erro na busca: {ex.Message}");
        }
    }
}
