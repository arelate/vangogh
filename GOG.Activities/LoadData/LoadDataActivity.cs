using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Status;

namespace GOG.Activities.Load
{
    public class LoadDataActivity : Activity
    {
        private ILoadAsyncDelegate[] loadDelegates;

        public LoadDataActivity(
            IStatusController statusController,
            params ILoadAsyncDelegate[] loadDelegates) :
            base(statusController)
        {
            this.loadDelegates = loadDelegates;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var loadDataTask = statusController.Create(status, "Load");
            for (var ii = 0; ii < loadDelegates.Length; ii++)
            {
                statusController.UpdateProgress(
                    loadDataTask,
                    ii + 1,
                    loadDelegates.Length,
                    "Data");

                await loadDelegates[ii].LoadAsync();
            }
            statusController.Complete(loadDataTask);
        }
    }
}
