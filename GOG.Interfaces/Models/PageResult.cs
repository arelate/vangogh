namespace GOG.Interfaces.Models
{
    public interface IPageResult
    {
        int Page { get; set; }
        int TotalPages { get; set; }
    }
}
