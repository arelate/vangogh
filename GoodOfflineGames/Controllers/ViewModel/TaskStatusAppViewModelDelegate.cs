using System;
using System.Collections.Generic;

using Interfaces.ViewModel;
using Interfaces.TaskStatus;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.ViewModel
{
    public class TaskStatusAppViewModelDelegate : IGetViewModelDelegate<ITaskStatus>
    {
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public TaskStatusAppViewModelDelegate(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public IDictionary<string, string> GetViewModel(ITaskStatus taskStatus)
        {
            if (taskStatus.Complete) return null;

            // viewmodel schemas
            var viewModel = new Dictionary<string, string>()
            {
                { "title", "" },
                { "containsProgress", "" },
                { "progressTarget", "" },
                { "progressPercent", "" },
                { "progressCurrent", "" },
                { "progressTotal", "" },
                { "containsEta", "" },
                { "remainingTime", "" },
                { "averageUnitsPerSecond", "" }
            };

            viewModel["title"] = taskStatus.Title;

            if (taskStatus.Progress != null)
            {
                var current = taskStatus.Progress.Current;
                var total = taskStatus.Progress.Total;

                viewModel["containsProgress"] = "true";
                viewModel["progressTarget"] = taskStatus.Progress.Target;
                viewModel["progressPercent"] = string.Format("{0:P1}", (double)current / total);

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                {
                    viewModel.Add("containsEta", "true");

                    currentFormatted = bytesFormattingController.Format(current);
                    totalFormatted = bytesFormattingController.Format(total);

                    var elapsed = DateTime.UtcNow - taskStatus.Started;
                    var unitsPerSecond = current / elapsed.TotalSeconds;
                    var speed = bytesFormattingController.Format((long)unitsPerSecond);
                    var remainingTime = secondsFormattingController.Format((long)((total - current) / unitsPerSecond));

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
