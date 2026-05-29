namespace Services.Dtos;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedResultDto<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var normalizedPageNumber = Math.Max(1, pageNumber);
        var normalizedPageSize = Math.Clamp(pageSize, 1, 100);
        var items = source.ToList();
        var totalCount = items.Count;

        return new PagedResultDto<T>
        {
            Items = items
                .Skip((normalizedPageNumber - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToList(),
            PageNumber = normalizedPageNumber,
            PageSize = normalizedPageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)normalizedPageSize)
        };
    }
}
