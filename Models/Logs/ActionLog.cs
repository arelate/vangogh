using Interfaces.Models.Logs;

namespace Models.Logs
{
    public class ActionLog: IActionLog
    {
        public string Title { get; set; }
        public int Progress { get; set; }
        public bool Complete { get; set; }
    }
}