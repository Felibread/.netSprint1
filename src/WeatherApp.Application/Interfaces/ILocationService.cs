using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Interfaces;

public interface ILocationService
{
    Task<Result<LocationDto>> CreateAsync(string name, double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<LocationDto>>> SearchAsync(string query, int limit = 20, CancellationToken cancellationToken = default);
}
