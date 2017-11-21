using System.Text;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Status;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.ViewUpdates;

namespace Controllers.ViewUpdates
{
    public class StatusGetViewUpdateDelegate : IGetViewUpdateDelegate<string[]>
    {
        private IStatus status;
        private ITemplateController templateController;
        private IGetViewModelDelegate<IStatus> statusViewModelDelegate;
        private ITreeToEnumerableController<IStatus> statusTreeToEnumerableController;

        private IList<string> viewParts;

        public StatusGetViewUpdateDelegate(
            IStatus status,
            ITemplateController templateController,
            IGetViewModelDelegate<IStatus> statusViewModelDelegate,
            ITreeToEnumerableController<IStatus> statusTreeToEnumerableController)
        {
            this.status = status;
            this.templateController = templateController;
            this.statusViewModelDelegate = statusViewModelDelegate;
            this.statusTreeToEnumerableController = statusTreeToEnumerableController;

            this.viewParts = new List<string>();
        }

        public string[] GetViewUpdate()
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
    }
}
