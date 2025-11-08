using WeatherApp.Api.ViewModels.Shared;

namespace WeatherApp.Api.ViewModels.Locations;

public class LocationListViewModel
{
    public LocationFilterViewModel Filter { get; set; } = new();
    public IReadOnlyList<LocationViewModel> Items { get; set; } = Array.Empty<LocationViewModel>();
    public PaginationViewModel Pagination { get; set; } = new(1, 10, 0, 0);
    public string? ErrorMessage { get; set; }
}

