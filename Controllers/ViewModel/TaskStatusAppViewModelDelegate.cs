using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Format;

using Interfaces.ViewModel;
using Interfaces.Status;

using Models.Units;

namespace Controllers.ViewModel
{
    public class StatusAppViewModelDelegate : IGetViewModelDelegate<IStatus>
    {
        static class StatusAppViewModelSchema
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

        readonly IFormatDelegate<IStatus, Tuple<long, double>> formatRemainingTimeAtSpeedDelegate;
        readonly IFormatDelegate<long, string> formatBytesDelegate;
        readonly IFormatDelegate<long, string> formatSecondsDelegate;

        public StatusAppViewModelDelegate(
            IFormatDelegate<IStatus, Tuple<long, double>> formatRemainingTimeAtSpeedDelegate,
            IFormatDelegate<long, string> formatBytesDelegate,
            IFormatDelegate<long, string> formatSecondsDelegate)
        {
            this.formatRemainingTimeAtSpeedDelegate = formatRemainingTimeAtSpeedDelegate;
            this.formatBytesDelegate = formatBytesDelegate;
            this.formatSecondsDelegate = formatSecondsDelegate;
        }

        public IDictionary<string, string> GetViewModel(IStatus status)
        {
            if (status.Complete) return null;

            // viewmodel schemas
            var viewModel = new Dictionary<string, string>
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

                    currentFormatted = formatBytesDelegate.Format(current);
                    totalFormatted = formatBytesDelegate.Format(total);

                    var estimatedTimeAvailable = formatRemainingTimeAtSpeedDelegate.Format(status);

                    var remainingTime = formatSecondsDelegate.Format(estimatedTimeAvailable.Item1);
                    var speed = formatBytesDelegate.Format((long)estimatedTimeAvailable.Item2);

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
