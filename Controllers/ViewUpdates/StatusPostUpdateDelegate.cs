using Interfaces.ViewUpdates;
using Interfaces.Output;

namespace Controllers.ViewUpdates
{
    public class StatusPostViewUpdateDelegate : IPostViewUpdateDelegate
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

        public void PostViewUpdate()
        {
            // TODO: fix this later
            outputOnRefreshDelegate.OutputOnRefresh(
                getViewUpdateDelegate.GetViewUpdateAsync().Result);
        }
    }
}
