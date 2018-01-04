using Interfaces.ViewUpdates;
using Interfaces.Output;

namespace Controllers.ViewUpdates
{
    public class StatusPostViewUpdateDelegate : IPostViewUpdateDelegate
    {
        private IGetViewUpdateDelegate<string[]> getViewUpdateDelegate;
        private IOutputOnRefreshDelegate<string[]> outputOnRefreshDelegate;

        public StatusPostViewUpdateDelegate(
            IGetViewUpdateDelegate<string[]> getViewUpdateDelegate,
            IOutputOnRefreshDelegate<string[]> outputOnRefreshDelegate)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.outputOnRefreshDelegate = outputOnRefreshDelegate;
        }

        public void PostViewUpdate()
        {
            outputOnRefreshDelegate.OutputOnRefresh(
                getViewUpdateDelegate.GetViewUpdate());
        }
    }
}
