using WeatherApp.Application.Common;

namespace WeatherApp.Api.Models.Hateoas;

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; }
    public PaginationMetadata Pagination { get; }
    public IReadOnlyList<LinkDto> Links { get; }

    private PagedResponse(IReadOnlyList<T> data, PaginationMetadata pagination, IReadOnlyList<LinkDto> links)
    {
        Data = data;
        Pagination = pagination;
        Links = links;
    }

    public static PagedResponse<T> FromPagedResult(PagedResult<T> pagedResult, IReadOnlyList<LinkDto> links)
    {
        var pagination = new PaginationMetadata(
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages);

        return new PagedResponse<T>(pagedResult.Items, pagination, links);
    }
}

public record PaginationMetadata(int PageNumber, int PageSize, int TotalItems, int TotalPages);

