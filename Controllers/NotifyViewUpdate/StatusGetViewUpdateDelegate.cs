using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Status;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.NotifyViewUpdate;

namespace Controllers.ViewUpdates
{
    public class GetStatusViewUpdateDelegate : IGetViewUpdateAsyncDelegate<string[]>
    {
        // readonly IStatus status;
        readonly ITemplateController templateController;
        readonly IGetViewModelDelegate<IStatus> statusViewModelDelegate;
        readonly IConvertDelegate<IStatus, IEnumerable<IStatus>> convertStatusTreeToEnumerableDelegate;

        readonly IList<string> viewParts;

        public GetStatusViewUpdateDelegate(
            // IStatus status,
            ITemplateController templateController,
            IGetViewModelDelegate<IStatus> statusViewModelDelegate,
            IConvertDelegate<IStatus, IEnumerable<IStatus>> convertStatusTreeToEnumerableDelegate)
        {
            // this.status = status;
            this.templateController = templateController;
            this.statusViewModelDelegate = statusViewModelDelegate;
            this.convertStatusTreeToEnumerableDelegate = convertStatusTreeToEnumerableDelegate;

            this.viewParts = new List<string>();
        }

        public async Task<string[]> GetViewUpdateAsync(IStatus status)
        {
            viewParts.Clear();
            foreach (var individualStatus in convertStatusTreeToEnumerableDelegate.Convert(status))
            {
                var viewModel = statusViewModelDelegate.GetViewModel(individualStatus);
                if (viewModel != null)
                {
                    var view = await templateController.BindAsync(
                        await templateController.GetPrimaryTemplateTitleAsync(status),
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
