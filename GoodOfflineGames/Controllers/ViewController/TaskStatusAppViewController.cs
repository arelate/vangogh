using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.Presentation;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;
using Interfaces.ViewController;

namespace Controllers.ViewController
{
    public class TaskStatusViewController : IViewController
    {
        private ITaskStatus appTaskStatus;
        private ITemplateController templateController;
        private IGetViewModelDelegate<ITaskStatus> taskStatusViewModelDelegate;
        private ITreeToEnumerableController<ITaskStatus> taskStatusTreeToEnumerableController;
        private IPresentationController<string> presentationController;

        public TaskStatusViewController(
            ITaskStatus appTaskStatus,
            ITemplateController templateController,
            IGetViewModelDelegate<ITaskStatus> taskStatusViewModelDelegate,
            ITreeToEnumerableController<ITaskStatus> taskStatusTreeToEnumerableController,
            IPresentationController<string> presentationController)
        {
            this.appTaskStatus = appTaskStatus;
            this.templateController = templateController;
            this.taskStatusViewModelDelegate = taskStatusViewModelDelegate;
            this.taskStatusTreeToEnumerableController = taskStatusTreeToEnumerableController;
            this.presentationController = presentationController;
        }

        private IEnumerable<string> CreateViews()
        {
            foreach (var taskStatus in taskStatusTreeToEnumerableController.ToEnumerable(appTaskStatus))
            {
                var viewModel = taskStatusViewModelDelegate.GetViewModel(taskStatus);
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
