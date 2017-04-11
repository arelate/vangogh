using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.ViewModel;
using Interfaces.Status;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.ViewModel
{
    public class StatusAppViewModelDelegate : IGetViewModelDelegate<IStatus>
    {
        private static class statusAppViewModelSchema
        {
            public const string Title = "title";
            public const string ContainsProgress = "containsProgress";
            public const string ProgressTarget = "progressTarget";
            public const string ProgressPercent = "progressPercent";
            public const string ProgressCurrent = "progressCurrent";
            public const string ProgressTotal = "progressTotal";
            public const string ContainsETA = "containsEta";
            public const string RemainingTime = "remainingTime";
            public const string AverageUnitsPerSecond = "averageUnitsPerSecond";
            public const string ContainsFailures = "containsFailures";
            public const string FailuresCount = "failuresCount";
            public const string ContainsWarnings = "containsWarnings";
            public const string WarningsCount = "warningsCount";
        }

        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public StatusAppViewModelDelegate(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public IDictionary<string, string> GetViewModel(IStatus status)
        {
            if (status.Complete) return null;

            // viewmodel schemas
            var viewModel = new Dictionary<string, string>()
            {
                { statusAppViewModelSchema.Title, "" },
                { statusAppViewModelSchema.ContainsProgress, "" },
                { statusAppViewModelSchema.ProgressTarget, "" },
                { statusAppViewModelSchema.ProgressPercent, "" },
                { statusAppViewModelSchema.ProgressCurrent, "" },
                { statusAppViewModelSchema.ProgressTotal, "" },
                { statusAppViewModelSchema.ContainsETA, "" },
                { statusAppViewModelSchema.RemainingTime, "" },
                { statusAppViewModelSchema.AverageUnitsPerSecond, "" },
                { statusAppViewModelSchema.ContainsFailures, ""},
                { statusAppViewModelSchema.FailuresCount, ""},
                { statusAppViewModelSchema.ContainsWarnings, ""},
                { statusAppViewModelSchema.WarningsCount, ""}
            };

            viewModel[statusAppViewModelSchema.Title] = status.Title;

            if (status.Progress != null)
            {
                var current = status.Progress.Current;
                var total = status.Progress.Total;

                viewModel[statusAppViewModelSchema.ContainsProgress] = "true";
                viewModel[statusAppViewModelSchema.ProgressTarget] = status.Progress.Target;
                viewModel[statusAppViewModelSchema.ProgressPercent] = string.Format("{0:P1}", (double)current / total);

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (status.Progress.Unit == DataUnits.Bytes)
                {
                    viewModel[statusAppViewModelSchema.ContainsETA] = "true";

                    currentFormatted = bytesFormattingController.Format(current);
                    totalFormatted = bytesFormattingController.Format(total);

                    var elapsed = DateTime.UtcNow - status.Started;
                    var unitsPerSecond = current / elapsed.TotalSeconds;
                    var speed = bytesFormattingController.Format((long)unitsPerSecond);
                    var remainingTime = secondsFormattingController.Format((long)((total - current) / unitsPerSecond));

                    viewModel[statusAppViewModelSchema.RemainingTime] = remainingTime;
                    viewModel[statusAppViewModelSchema.AverageUnitsPerSecond] = speed;
                }

                viewModel[statusAppViewModelSchema.ProgressCurrent] = currentFormatted;
                viewModel[statusAppViewModelSchema.ProgressTotal] = totalFormatted;
            }

            if (status.Failures != null && status.Failures.Any())
            {
                viewModel[statusAppViewModelSchema.ContainsFailures] = "true";
                viewModel[statusAppViewModelSchema.FailuresCount] = status.Failures.Count.ToString();
            }

            if (status.Warnings != null && status.Warnings.Any())
            {
                viewModel[statusAppViewModelSchema.ContainsWarnings] = "true";
                viewModel[statusAppViewModelSchema.WarningsCount] = status.Warnings.Count.ToString();
            }

            return viewModel;
        }
    }
}
