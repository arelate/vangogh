using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.ViewModel;
using Interfaces.TaskStatus;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.ViewModel
{
    public class TaskStatusGetViewModelDelegate : IGetViewModelDelegate<ITaskStatus>
    {
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public TaskStatusGetViewModelDelegate(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
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
                viewModel.Add("progressPercent", 
                    string.Format(
                        "{0:P1}", 
                        (double)taskStatus.Progress.Current / taskStatus.Progress.Total));

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
