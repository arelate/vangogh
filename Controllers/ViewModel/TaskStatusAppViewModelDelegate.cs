using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.ViewModel;
using Interfaces.Status;
using Interfaces.Formatting;
using Interfaces.StatusRemainingTime;

using Models.Units;

namespace Controllers.ViewModel
{
    public class StatusAppViewModelDelegate : IGetViewModelDelegate<IStatus>
    {
        private static class StatusAppViewModelSchema
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

        private IGetRemainingTimeAtUnitsPerSecondDelegate getRemainingTimeAtUnitsPerSecondDelegate;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public StatusAppViewModelDelegate(
            IGetRemainingTimeAtUnitsPerSecondDelegate getRemainingTimeAtUnitsPerSecondDelegate,
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.getRemainingTimeAtUnitsPerSecondDelegate = getRemainingTimeAtUnitsPerSecondDelegate;
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public IDictionary<string, string> GetViewModel(IStatus status)
        {
            if (status.Complete) return null;

            // viewmodel schemas
            var viewModel = new Dictionary<string, string>()
            {
                { StatusAppViewModelSchema.Title, "" },
                { StatusAppViewModelSchema.ContainsProgress, "" },
                { StatusAppViewModelSchema.ProgressTarget, "" },
                { StatusAppViewModelSchema.ProgressPercent, "" },
                { StatusAppViewModelSchema.ProgressCurrent, "" },
                { StatusAppViewModelSchema.ProgressTotal, "" },
                { StatusAppViewModelSchema.ContainsETA, "" },
                { StatusAppViewModelSchema.RemainingTime, "" },
                { StatusAppViewModelSchema.AverageUnitsPerSecond, "" },
                { StatusAppViewModelSchema.ContainsFailures, ""},
                { StatusAppViewModelSchema.FailuresCount, ""},
                { StatusAppViewModelSchema.ContainsWarnings, ""},
                { StatusAppViewModelSchema.WarningsCount, ""}
            };

            viewModel[StatusAppViewModelSchema.Title] = status.Title;

            if (status.Progress != null)
            {
                var current = status.Progress.Current;
                var total = status.Progress.Total;

                viewModel[StatusAppViewModelSchema.ContainsProgress] = "true";
                viewModel[StatusAppViewModelSchema.ProgressTarget] = status.Progress.Target;
                viewModel[StatusAppViewModelSchema.ProgressPercent] = string.Format("{0:P1}", (double)current / total);

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (status.Progress.Unit == DataUnits.Bytes)
                {
                    viewModel[StatusAppViewModelSchema.ContainsETA] = "true";

                    currentFormatted = bytesFormattingController.Format(current);
                    totalFormatted = bytesFormattingController.Format(total);

                    var remainingTimeAtSpeed = getRemainingTimeAtUnitsPerSecondDelegate.GetRemainingTimeAtUnitsPerSecond(status);

                    var remainingTime = secondsFormattingController.Format(remainingTimeAtSpeed.Item1);
                    var speed = bytesFormattingController.Format((long)remainingTimeAtSpeed.Item2);

                    viewModel[StatusAppViewModelSchema.RemainingTime] = remainingTime;
                    viewModel[StatusAppViewModelSchema.AverageUnitsPerSecond] = speed;
                }

                viewModel[StatusAppViewModelSchema.ProgressCurrent] = currentFormatted;
                viewModel[StatusAppViewModelSchema.ProgressTotal] = totalFormatted;
            }

            if (status.Failures != null && status.Failures.Any())
            {
                viewModel[StatusAppViewModelSchema.ContainsFailures] = "true";
                viewModel[StatusAppViewModelSchema.FailuresCount] = status.Failures.Count.ToString();
            }

            if (status.Warnings != null && status.Warnings.Any())
            {
                viewModel[StatusAppViewModelSchema.ContainsWarnings] = "true";
                viewModel[StatusAppViewModelSchema.WarningsCount] = status.Warnings.Count.ToString();
            }

            return viewModel;
        }
    }
}
