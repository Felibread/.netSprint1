namespace WeatherApp.Api.ViewModels.Shared;

public record PaginationViewModel(
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages)
{
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

