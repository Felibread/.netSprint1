using WeatherApp.Api.ViewModels.Locations;

namespace WeatherApp.Api.ViewModels.Home;

public class HomeViewModel
{
    public IReadOnlyList<LocationViewModel> HighlightedLocations { get; init; } = Array.Empty<LocationViewModel>();
    public int TotalLocations { get; init; }
    public int TotalAlertsEvaluated { get; init; }
    public string? Message { get; init; }
}

