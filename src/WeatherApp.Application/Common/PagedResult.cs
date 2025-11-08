namespace WeatherApp.Application.Common;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }

    public PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalItems)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "O número da página deve ser maior ou igual a 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser maior ou igual a 1.");
        if (totalItems < 0)
            throw new ArgumentOutOfRangeException(nameof(totalItems), "O total de itens não pode ser negativo.");

        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}

