using System.Text;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Presentation;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.ViewController;

namespace Controllers.ViewController
{
    public class StatusViewController : IViewController<string>
    {
        private IStatus status;
        private ITemplateController templateController;
        private IGetViewModelDelegate<IStatus> statusViewModelDelegate;
        private ITreeToEnumerableController<IStatus> statusTreeToEnumerableController;
        private IPresentationController<string> presentationController;

        public StatusViewController(
            IStatus status,
            ITemplateController templateController,
            IGetViewModelDelegate<IStatus> statusViewModelDelegate,
            ITreeToEnumerableController<IStatus> statusTreeToEnumerableController,
            IPresentationController<string> presentationController)
        {
            this.status = status;
            this.templateController = templateController;
            this.statusViewModelDelegate = statusViewModelDelegate;
            this.statusTreeToEnumerableController = statusTreeToEnumerableController;
            this.presentationController = presentationController;
        }

        public string RequestUpdatedView()
        {
            var viewStringBuilder = new StringBuilder();
            foreach (var individualStatus in statusTreeToEnumerableController.ToEnumerable(status))
            {
                var viewModel = statusViewModelDelegate.GetViewModel(individualStatus);
                if (viewModel != null)
                {
                    var view = templateController.Bind(
                        templateController.PrimaryTemplate,
                        viewModel);
                    if (!string.IsNullOrEmpty(view) &&
                        !string.IsNullOrWhiteSpace(view))
                        viewStringBuilder.Append(view);
                }
            }

            return viewStringBuilder.ToString();
        }

        public void PostUpdateNotification()
        {
            presentationController.Present(RequestUpdatedView());
        }

        public async Task PostUpdateNotificationAsync()
        {
            await presentationController.PresentAsync(RequestUpdatedView());
        }
    }
}
