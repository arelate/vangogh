namespace Interfaces.Models.Logs
{
    public interface IActionLog
    {
        string Title { get; set; }
        int Progress { get; set; }
        bool Complete { get; set; }
    }
}