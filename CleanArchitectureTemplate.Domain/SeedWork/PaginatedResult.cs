namespace CleanArchitectureTemplate.Domain.SeedWork;

public sealed class PaginatedResult<T>
{
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalRecords { get; private set; }
    public IEnumerable<T> Items { get; private set; }

    public PaginatedResult(
        int pageNumber,
        int pageSize,
        int totalRecords,
        IEnumerable<T> items)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        Items = items;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    }
}

