using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Presentation;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.ViewController;

namespace Controllers.ViewController
{
    public class StatusViewController : IViewController
    {
        private IStatus appstatus;
        private ITemplateController templateController;
        private IGetViewModelDelegate<IStatus> statusViewModelDelegate;
        private ITreeToEnumerableController<IStatus> statusTreeToEnumerableController;
        private IPresentationController<string> presentationController;

        public StatusViewController(
            IStatus appstatus,
            ITemplateController templateController,
            IGetViewModelDelegate<IStatus> statusViewModelDelegate,
            ITreeToEnumerableController<IStatus> statusTreeToEnumerableController,
            IPresentationController<string> presentationController)
        {
            this.appstatus = appstatus;
            this.templateController = templateController;
            this.statusViewModelDelegate = statusViewModelDelegate;
            this.statusTreeToEnumerableController = statusTreeToEnumerableController;
            this.presentationController = presentationController;
        }

        private IEnumerable<string> CreateViews()
        {
            foreach (var status in statusTreeToEnumerableController.ToEnumerable(appstatus))
            {
                var viewModel = statusViewModelDelegate.GetViewModel(status);
                if (viewModel != null)
                {
                    var view = templateController.Bind(
                        templateController.PrimaryTemplate,
                        viewModel);
                    yield return view;
                }
            }
        }

        public void PresentViews()
        {
            presentationController.Present(CreateViews());
        }

        public async Task PresentViewsAsync()
        {
            await presentationController.PresentAsync(CreateViews());
        }
    }
}
