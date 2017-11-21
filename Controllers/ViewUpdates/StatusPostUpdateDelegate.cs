using Interfaces.ViewUpdates;
using Interfaces.Presentation;

namespace Controllers.ViewUpdates
{
    public class StatusPostUpdateDelegate : IPostViewUpdateDelegate
    {
        private IGetViewUpdateDelegate<string[]> getViewUpdateDelegate;
        private IPresentNewDelegate<string[]> presentNewDelegate;

        public StatusPostUpdateDelegate(
            IGetViewUpdateDelegate<string[]> getViewUpdateDelegate,
            IPresentNewDelegate<string[]> presentNewDelegate)
        {
            this.getViewUpdateDelegate = getViewUpdateDelegate;
            this.presentNewDelegate = presentNewDelegate;
        }

        public void PostViewUpdate()
        {
            presentNewDelegate.PresentNew(
                getViewUpdateDelegate.GetViewUpdate());
        }
    }
}
