using System.Threading.Tasks;

using Interfaces.Controllers.Output;

using Interfaces.NotifyViewUpdate;

using Interfaces.Status;

namespace Controllers.ViewUpdates
{
    public class NotifyStatusViewUpdateController: INotifyViewUpdateController
    {
        readonly IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate;
        readonly IOutputController<string[]> outputController;

        public NotifyStatusViewUpdateController(
            IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate,
            IOutputController<string[]> outputController)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.outputController = outputController;
        }

        public async Task NotifyViewUpdateOutputOnRefreshAsync(IStatus status)
        {
            await outputController.OutputOnRefreshAsync(
                await getViewUpdateDelegate.GetViewUpdateAsync(status));
        }

        public async Task NotifyViewUpdateOutputContinuousAsync(IStatus status)
        {
            await outputController.OutputContinuousAsync(
                null,
                await getViewUpdateDelegate.GetViewUpdateAsync(status));
        }
    }
}
