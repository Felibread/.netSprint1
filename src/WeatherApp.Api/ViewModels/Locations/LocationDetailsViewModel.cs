using WeatherApp.Api.ViewModels.Alerts;
using WeatherApp.Api.ViewModels.Weather;

namespace WeatherApp.Api.ViewModels.Locations;

public class LocationDetailsViewModel
{
    public LocationViewModel Location { get; init; } = new(Guid.Empty, string.Empty, 0, 0);
    public WeatherReadingViewModel? CurrentWeather { get; init; }
    public IReadOnlyList<AlertViewModel> Alerts { get; init; } = Array.Empty<AlertViewModel>();
    public string? ErrorMessage { get; init; }
}

