using System.Threading.Tasks;

using Interfaces.Controllers.Output;

using Interfaces.NotifyViewUpdate;

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

        public async Task NotifyViewUpdateOutputOnRefreshAsync()
        {
            await outputController.OutputOnRefreshAsync(
                await getViewUpdateDelegate.GetViewUpdateAsync());
        }

        public async Task NotifyViewUpdateOutputContinuousAsync()
        {
            await outputController.OutputContinuousAsync(
                null,
                await getViewUpdateDelegate.GetViewUpdateAsync());
        }
    }
}
