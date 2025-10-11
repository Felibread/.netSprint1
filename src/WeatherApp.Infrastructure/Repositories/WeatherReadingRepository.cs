using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
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
            .Where(r => r.Location.Id == locationId)
            .OrderByDescending(r => r.ObservedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetHourlyForLocationAsync(Guid locationId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WeatherReadings
            .Include(r => r.Location)
            .Where(r => r.Location.Id == locationId && r.ObservedAt >= from && r.ObservedAt <= to)
            .OrderBy(r => r.ObservedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
