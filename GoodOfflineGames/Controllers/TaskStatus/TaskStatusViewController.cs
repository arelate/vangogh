using System;
using System.Collections.Generic;

using Interfaces.TaskStatus;
using Interfaces.Formatting;
using Interfaces.Presentation;
using Interfaces.Tree;
using Interfaces.Template;

using Models.Units;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController : ITaskStatusViewController
    {
        private ITaskStatus applicationTaskStatus;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;
        private ITemplateController templateController;
        private ITreeToListController<ITaskStatus> taskStatusTreeToListController;
        private IPresentationController<string> consolePresentationController;

        public TaskStatusViewController(
            ITaskStatus applicationTaskStatus,
            ITemplateController templateController,
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController,
            ITreeToListController<ITaskStatus> taskStatusTreeToListController,
            IPresentationController<string> consolePresentationController)
        {
            this.applicationTaskStatus = applicationTaskStatus;

            this.templateController = templateController;

            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;

            this.taskStatusTreeToListController = taskStatusTreeToListController;

            this.consolePresentationController = consolePresentationController;
        }

        public void CreateView(bool overrideThrottling = false)
        {
            var views = new List<string>();

            var taskStatusList = taskStatusTreeToListController.ToList(applicationTaskStatus);

            foreach (var taskStatus in taskStatusList)
            {
                var viewModel = GetViewModel(taskStatus);
                if (viewModel != null)
                {
                    var view = templateController.Bind("taskStatus", viewModel);
                    views.Add(view);
                }
            }

            consolePresentationController.Present(views, overrideThrottling);
        }

        public IDictionary<string, string> GetViewModel(ITaskStatus taskStatus)
        {
            if (taskStatus.Complete) return null;

            var viewModel = new Dictionary<string, string>();

            viewModel.Add("title", taskStatus.Title);

            if (taskStatus.Progress != null)
            {
                viewModel.Add("containsProgress", "true");
                viewModel.Add("progressTarget", taskStatus.Progress.Target);
                viewModel.Add("progressPercent", string.Format("{0:P1}", (double)taskStatus.Progress.Current / taskStatus.Progress.Total));

                var currentFormatted = taskStatus.Progress.Current.ToString();
                var totalFormatted = taskStatus.Progress.Total.ToString();

                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                {
                    viewModel.Add("containsEta", "true");

                    currentFormatted = bytesFormattingController.Format(taskStatus.Progress.Current);
                    totalFormatted = bytesFormattingController.Format(taskStatus.Progress.Total);

                    var elapsed = DateTime.UtcNow - taskStatus.Started;
                    var unitsPerSecond = taskStatus.Progress.Current / elapsed.TotalSeconds;
                    var speed = bytesFormattingController.Format((long)unitsPerSecond);
                    var remainingTime = secondsFormattingController.Format((long)((taskStatus.Progress.Total - taskStatus.Progress.Current) / unitsPerSecond));

                    viewModel.Add("remainingTime", remainingTime);
                    viewModel.Add("averageUnitsPerSecond", speed);
                }

                viewModel.Add("progressCurrent", currentFormatted);
                viewModel.Add("progressTotal", totalFormatted);
            }

            return viewModel;
        }
    }
}
