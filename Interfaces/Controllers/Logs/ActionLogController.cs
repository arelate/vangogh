using Interfaces.Models.Logs;

namespace Interfaces.Controllers.Logs
{
    public interface IStartActionDelegate
    {
        void StartAction(string title);
    }

    public interface ISetActionTargetDelegate
    {
        void SetActionTarget(int target);
    }

    public interface IIncrementActionProgressDelegate
    {
        void IncrementActionProgress(int increment = 1);
    }

    public interface ICompleteActionDelegate
    {
        void CompleteAction();
    }

    public interface IActionLogController:
        IStartActionDelegate,
        ISetActionTargetDelegate,
        IIncrementActionProgressDelegate,
        ICompleteActionDelegate
    {
        // ...
    }
}