using Interfaces.Models.Logs;

namespace Interfaces.Controllers.Logs
{
    public interface IOpenResponseLogDelegate
    {
        void OpenResponseLog(string title);
    }

    public interface ICloseResponseLogDelegate
    {
        IResponseLog CloseResponseLog();
    }

    public interface IResponseLogController :
        IActionLogController,
        IOpenResponseLogDelegate,
        ICloseResponseLogDelegate
    {
        // ...
    }
}