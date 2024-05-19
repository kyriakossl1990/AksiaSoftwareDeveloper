namespace AksiaSoftwareDeveloper.Common;

public class PaginatedResponse<T> where
    T : class
{
    public int TotalPage { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Entities { get; set; } = [];
}
