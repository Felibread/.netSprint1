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

    public async Task<IReadOnlyList<Location>> SearchByNameAsync(string nameQuery, int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = nameQuery.Trim().ToLowerInvariant();
        return await _dbContext.Locations
            .Where(l => EF.Functions.Like(l.Name.ToLower(), $"%{query}%"))
            .OrderBy(l => l.Name)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        _dbContext.Locations.Update(location);
        return Task.CompletedTask;
    }
}
