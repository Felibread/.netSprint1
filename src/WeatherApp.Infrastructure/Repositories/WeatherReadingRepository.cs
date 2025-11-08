using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;
using WeatherApp.Domain.Repositories;
using WeatherApp.Infrastructure.Persistence;

namespace WeatherApp.Infrastructure.Repositories;

public class WeatherReadingRepository : IWeatherReadingRepository
{
    private readonly AppDbContext _dbContext;

    public WeatherReadingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(WeatherReading reading, CancellationToken cancellationToken = default)
    {
        await _dbContext.WeatherReadings.AddAsync(reading, cancellationToken);
    }

    public async Task<WeatherReading?> GetLatestForLocationAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WeatherReadings
            .Include(r => r.Location)
            .Where(r => r.LocationId == locationId)
            .OrderByDescending(r => r.ObservedAt.ToUnixTimeMilliseconds())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetHourlyForLocationAsync(Guid locationId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WeatherReadings
            .Include(r => r.Location)
            .Where(r => r.LocationId == locationId && r.ObservedAt >= from && r.ObservedAt <= to)
            .OrderBy(r => r.ObservedAt.ToUnixTimeMilliseconds())
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<WeatherReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WeatherReadings
            .Include(r => r.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<WeatherReading?> GetTrackedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WeatherReadings
            .Include(r => r.Location)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<WeatherReading> Items, int TotalCount)> SearchAsync(
        Guid? locationId,
        DateTimeOffset? observedFrom,
        DateTimeOffset? observedTo,
        WeatherCondition? condition,
        double? minTemperature,
        double? maxTemperature,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool ascending,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.WeatherReadings
            .Include(r => r.Location)
            .AsQueryable();

        if (locationId.HasValue)
        {
            queryable = queryable.Where(r => r.LocationId == locationId.Value);
        }

        if (observedFrom.HasValue)
        {
            queryable = queryable.Where(r => r.ObservedAt >= observedFrom.Value);
        }

        if (observedTo.HasValue)
        {
            queryable = queryable.Where(r => r.ObservedAt <= observedTo.Value);
        }

        if (condition.HasValue)
        {
            queryable = queryable.Where(r => r.Condition == condition.Value);
        }

        if (minTemperature.HasValue)
        {
            queryable = queryable.Where(r => r.Temperature >= minTemperature.Value);
        }

        if (maxTemperature.HasValue)
        {
            queryable = queryable.Where(r => r.Temperature <= maxTemperature.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = (sortBy?.ToLowerInvariant()) switch
        {
            "temperature" => ascending ? queryable.OrderBy(r => r.Temperature) : queryable.OrderByDescending(r => r.Temperature),
            "observedat" => ascending ? queryable.OrderBy(r => r.ObservedAt.ToUnixTimeMilliseconds()) : queryable.OrderByDescending(r => r.ObservedAt.ToUnixTimeMilliseconds()),
            "humidity" => ascending ? queryable.OrderBy(r => r.HumidityPercent) : queryable.OrderByDescending(r => r.HumidityPercent),
            _ => ascending ? queryable.OrderBy(r => r.ObservedAt.ToUnixTimeMilliseconds()) : queryable.OrderByDescending(r => r.ObservedAt.ToUnixTimeMilliseconds())
        };

        var items = await queryable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task UpdateAsync(WeatherReading reading, CancellationToken cancellationToken = default)
    {
        _dbContext.WeatherReadings.Update(reading);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.WeatherReadings.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            _dbContext.WeatherReadings.Remove(entity);
        }
    }
}
