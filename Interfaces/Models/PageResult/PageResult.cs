namespace Interfaces.Models.PageResult
{
    public interface IPageResult
    {
        int Page { get; set; }
        int TotalPages { get; set; }
    }
}