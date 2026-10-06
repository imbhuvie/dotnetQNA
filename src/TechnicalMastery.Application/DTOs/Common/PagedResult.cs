namespace TechnicalMastery.Application.DTOs.Common;

/// <summary>
/// A single page of results with total counts for client-side paging UI.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
