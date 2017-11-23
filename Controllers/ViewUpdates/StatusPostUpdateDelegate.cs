using Interfaces.ViewUpdates;
using Interfaces.Output;

namespace Controllers.ViewUpdates
{
    public class StatusPostUpdateDelegate : IPostViewUpdateDelegate
    {
        private IGetViewUpdateDelegate<string[]> getViewUpdateDelegate;
        private IOutputOnRefreshDelegate<string[]> outputOnRefreshDelegate;

        public StatusPostUpdateDelegate(
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
