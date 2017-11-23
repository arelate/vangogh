using System.Threading.Tasks;

using Interfaces.ViewUpdates;
using Interfaces.Output;

namespace Controllers.ViewUpdates
{
    public class StatusPostUpdateAsyncDelegate : IPostViewUpdateAsyncDelegate
    {
        private IGetViewUpdateDelegate<string[]> getViewUpdateDelegate;
        private IOutputContinuousAsyncDelegate<string[]> outputContinuousAsync;

        public StatusPostUpdateAsyncDelegate(
            IGetViewUpdateDelegate<string[]> getViewUpdateDelegate,
            IOutputContinuousAsyncDelegate<string[]> outputContinuousAsync)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.outputContinuousAsync = outputContinuousAsync;
        }

        public async Task PostViewUpdateAsync()
        {
            await outputContinuousAsync.OutputContinuousAsync(
                getViewUpdateDelegate.GetViewUpdate());
        }
    }
}
