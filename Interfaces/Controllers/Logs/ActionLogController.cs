using Interfaces.Models.Logs;

namespace Interfaces.Controllers.Logs
{
    public interface IStartActionDelegate
    {
        void StartAction(string title);
    }

    public interface ISetActionProgressDelegate
    {
        void SetActionProgress(int progress);
    }

    public interface IGetActionProgressPercentDelegate
    {
        double GetActionProgressPercent(int total);
    }

    public interface IIncrementActionProgressDelegate
    {
        void IncrementActionProgress();
    }

    public interface ICompleteActionDelegate
    {
        void CompleteAction();
    }

    public interface IActionLogController:
        IStartActionDelegate,
        ISetActionProgressDelegate,
        IGetActionProgressPercentDelegate,
        IIncrementActionProgressDelegate,
        ICompleteActionDelegate
    {
        // ...
    }
}