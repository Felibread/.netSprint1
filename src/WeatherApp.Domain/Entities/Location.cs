using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.Entities;

public class Location
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public Coordinates Coordinates { get; private set; }

    public Location(string name, Coordinates coordinates)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name is required.", nameof(name));

        Name = name.Trim();
        Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
    }
}
