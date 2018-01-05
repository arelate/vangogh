using System.Threading.Tasks;

using Interfaces.ViewUpdates;
using Interfaces.Output;

namespace Controllers.ViewUpdates
{
    public class StatusPostViewUpdateDelegate : IPostViewUpdateAsyncDelegate
    {
        private IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate;
        private IOutputOnRefreshDelegate<string[]> outputOnRefreshDelegate;

        public StatusPostViewUpdateDelegate(
            IGetViewUpdateAsyncDelegate<string[]> getViewUpdateDelegate,
            IOutputOnRefreshDelegate<string[]> outputOnRefreshDelegate)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.outputOnRefreshDelegate = outputOnRefreshDelegate;
        }

        public async Task PostViewUpdateAsync()
        {
            outputOnRefreshDelegate.OutputOnRefresh(
                await getViewUpdateDelegate.GetViewUpdateAsync());
        }
    }
}
