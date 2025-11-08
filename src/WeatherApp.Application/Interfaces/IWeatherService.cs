using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.Interfaces;

public interface IWeatherService
{
    Task<Result<WeatherReadingDto>> GetCurrentAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<Result<WeatherReadingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<WeatherReadingDto>>> SearchAsync(
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
        CancellationToken cancellationToken = default);
    Task<Result<WeatherReadingDto>> CreateAsync(WeatherReadingUpsertDto dto, CancellationToken cancellationToken = default);
    Task<Result<WeatherReadingDto>> UpdateAsync(Guid id, WeatherReadingUpsertDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
