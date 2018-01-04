using System.Threading.Tasks;

using Interfaces.ViewUpdates;
using Interfaces.Output;
using Interfaces.Status;

namespace Controllers.ViewUpdates
{
    public class StatusPostUpdateAsyncDelegate : IPostViewUpdateAsyncDelegate
    {
        private IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate;
        private IOutputContinuousAsyncDelegate<string[]> outputContinuousAsync;

        public StatusPostUpdateAsyncDelegate(
            IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate,
            IOutputContinuousAsyncDelegate<string[]> outputContinuousAsync)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.outputContinuousAsync = outputContinuousAsync;
        }

        public async Task PostViewUpdateAsync()
        {
            await outputContinuousAsync.OutputContinuousAsync(
                await getViewUpdateDelegate.GetViewUpdateAsync());
        }
    }
}
