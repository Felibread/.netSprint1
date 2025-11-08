using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Repositories;
using WeatherApp.Infrastructure.Persistence;

namespace WeatherApp.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _dbContext;

    public LocationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Locations.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            _dbContext.Locations.Remove(entity);
        }
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Location?> GetTrackedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Location> Items, int TotalCount)> SearchAsync(
        string? nameQuery,
        double? minLatitude,
        double? maxLatitude,
        double? minLongitude,
        double? maxLongitude,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool ascending,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Locations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameQuery))
        {
            var normalized = nameQuery.Trim();
            queryable = queryable.Where(l => EF.Functions.Like(l.Name, $"%{normalized}%"));
        }

        if (minLatitude.HasValue)
            queryable = queryable.Where(l => l.Coordinates.Latitude >= minLatitude.Value);
        if (maxLatitude.HasValue)
            queryable = queryable.Where(l => l.Coordinates.Latitude <= maxLatitude.Value);
        if (minLongitude.HasValue)
            queryable = queryable.Where(l => l.Coordinates.Longitude >= minLongitude.Value);
        if (maxLongitude.HasValue)
            queryable = queryable.Where(l => l.Coordinates.Longitude <= maxLongitude.Value);

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = (sortBy?.ToLower()) switch
        {
            "latitude" => ascending ? queryable.OrderBy(l => l.Coordinates.Latitude) : queryable.OrderByDescending(l => l.Coordinates.Latitude),
            "longitude" => ascending ? queryable.OrderBy(l => l.Coordinates.Longitude) : queryable.OrderByDescending(l => l.Coordinates.Longitude),
            _ => ascending ? queryable.OrderBy(l => l.Name) : queryable.OrderByDescending(l => l.Name)
        };

        var items = await queryable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        _dbContext.Locations.Update(location);
        return Task.CompletedTask;
    }
}
