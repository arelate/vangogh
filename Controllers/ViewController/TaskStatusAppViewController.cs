using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Presentation;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.ViewController;

using Models.Separators;

namespace Controllers.ViewController
{
    public class StatusViewController : IViewController<string[]>
    {
        private IStatus status;
        private ITemplateController templateController;
        private IGetViewModelDelegate<IStatus> statusViewModelDelegate;
        private ITreeToEnumerableController<IStatus> statusTreeToEnumerableController;
        private IPresentationController<string[]> presentationController;

        private IList<string> viewParts;
        //private const string viewPartsSeparator = Separators.Common.Space + Separators.Common.MoreThan + Separators.Common.Space;

        public StatusViewController(
            IStatus status,
            ITemplateController templateController,
            IGetViewModelDelegate<IStatus> statusViewModelDelegate,
            ITreeToEnumerableController<IStatus> statusTreeToEnumerableController,
            IPresentationController<string[]> presentationController)
        {
            this.status = status;
            this.templateController = templateController;
            this.statusViewModelDelegate = statusViewModelDelegate;
            this.statusTreeToEnumerableController = statusTreeToEnumerableController;
            this.presentationController = presentationController;

            this.viewParts = new List<string>();
        }

        public string[] RequestUpdatedView()
        {
            viewParts.Clear();
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
                        viewParts.Add(view);
                }
            }

            return viewParts.ToArray();
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
