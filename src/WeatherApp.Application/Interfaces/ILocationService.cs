using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Interfaces;

public interface ILocationService
{
    Task<Result<LocationDto>> CreateAsync(string name, double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<Result<LocationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<LocationDto>> UpdateAsync(Guid id, string name, double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<LocationDto>>> SearchAsync(
        string? name,
        double? minLatitude,
        double? maxLatitude,
        double? minLongitude,
        double? maxLongitude,
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);
}
