using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.NotifyViewUpdate;

namespace Controllers.ViewUpdates
{
    public class GetStatusViewUpdateDelegate : IGetViewUpdateAsyncDelegate<string[]>
    {
        private IStatus status;
        private ITemplateController templateController;
        private IGetViewModelDelegate<IStatus> statusViewModelDelegate;
        private ITreeToEnumerableController<IStatus> statusTreeToEnumerableController;

        private IList<string> viewParts;

        public GetStatusViewUpdateDelegate(
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

        public async Task<string[]> GetViewUpdateAsync()
        {
            viewParts.Clear();
            foreach (var individualStatus in statusTreeToEnumerableController.ToEnumerable(status))
            {
                var viewModel = statusViewModelDelegate.GetViewModel(individualStatus);
                if (viewModel != null)
                {
                    var view = await templateController.BindAsync(
                        templateController.PrimaryTemplate,
                        viewModel,
                        status);

                    if (!string.IsNullOrEmpty(view) &&
                        !string.IsNullOrWhiteSpace(view))
                        viewParts.Add(view);
                }
            }

            return viewParts.ToArray();
        }
    }
}
