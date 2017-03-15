using System.Collections.Generic;

using Interfaces.TaskStatus;
using Interfaces.Presentation;
using Interfaces.Tree;
using Interfaces.Template;
using Interfaces.ViewModel;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController : ITaskStatusViewController
    {
        private ITaskStatus applicationTaskStatus;

        private ITemplateController templateController;
        private IGetViewModelDelegate<ITaskStatus> taskStatusViewModelDelegate;

        private ITreeToListController<ITaskStatus> taskStatusTreeToListController;
        private IPresentationController<string> consolePresentationController;

        public TaskStatusViewController(
            ITaskStatus applicationTaskStatus,
            ITemplateController templateController,
            IGetViewModelDelegate<ITaskStatus> taskStatusViewModelDelegate,
            ITreeToListController<ITaskStatus> taskStatusTreeToListController,
            IPresentationController<string> consolePresentationController)
        {
            this.applicationTaskStatus = applicationTaskStatus;
            this.templateController = templateController;
            this.taskStatusViewModelDelegate = taskStatusViewModelDelegate;
            this.taskStatusTreeToListController = taskStatusTreeToListController;
            this.consolePresentationController = consolePresentationController;
        }

        public void CreateView(bool overrideThrottling = false)
        {
            var views = new List<string>();

            var taskStatusList = taskStatusTreeToListController.ToList(applicationTaskStatus);

            foreach (var taskStatus in taskStatusList)
            {
                var viewModel = taskStatusViewModelDelegate.GetViewModel(taskStatus);
                if (viewModel != null)
                {
                    var view = templateController.Bind("taskStatus", viewModel);
                    views.Add(view);
                }
            }

            consolePresentationController.Present(views, overrideThrottling);
        }
    }
}
