using System.Threading.Tasks;

using Interfaces.NotifyViewUpdate;
using Interfaces.Output;

namespace Controllers.ViewUpdates
{
    public class NotifyStatusViewUpdateController: INotifyViewUpdateController
    {
        private IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate;
        private IOutputController<string[]> outputController;

        public NotifyStatusViewUpdateController(
            IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate,
            IOutputController<string[]> outputController)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.outputController = outputController;
        }

        public async Task NotifyViewUpdateOutputOnRefreshAsync()
        {
            await outputController.OutputOnRefreshAsync(
                await getViewUpdateDelegate.GetViewUpdateAsync());
        }

        public async Task NotifyViewUpdateOutputContinuousAsync()
        {
            await outputController.OutputContinuousAsync(
                await getViewUpdateDelegate.GetViewUpdateAsync());
        }
    }
}
