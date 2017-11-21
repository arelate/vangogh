using System.Threading.Tasks;

using Interfaces.ViewUpdates;
using Interfaces.Presentation;

namespace Controllers.ViewUpdates
{
    public class StatusPostUpdateAsyncDelegate : IPostViewUpdateAsyncDelegate
    {
        private IGetViewUpdateDelegate<string[]> getViewUpdateDelegate;
        private IPresentAsyncDelegate<string[]> presentAsyncDelegate;

        public StatusPostUpdateAsyncDelegate(
            IGetViewUpdateDelegate<string[]> getViewUpdateDelegate,
            IPresentAsyncDelegate<string[]> presentAsyncDelegate)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.presentAsyncDelegate = presentAsyncDelegate;
        }

        public async Task PostViewUpdateAsync()
        {
            await presentAsyncDelegate.PresentAsync(
                getViewUpdateDelegate.GetViewUpdate());
        }
    }
}
