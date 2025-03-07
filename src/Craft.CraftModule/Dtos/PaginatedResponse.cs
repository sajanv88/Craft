namespace Craft.CraftModule.Dtos;

/// <summary>
///
/// </summary>
public sealed class PaginatedResponse<I>
{
    public IReadOnlyList<I> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => TotalPages != 1 && CurrentPage < TotalPages;
}
