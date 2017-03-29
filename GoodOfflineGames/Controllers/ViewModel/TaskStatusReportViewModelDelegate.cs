using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.ViewModel;
using Interfaces.Status;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.ViewModel
{
    public class StatusReportViewModelDelegate : IGetViewModelDelegate<IStatus>
    {
        private static class statusReportViewModelSchema
        {
            public const string Complete = "complete";
            public const string Title = "title";
            public const string Started = "started";
            public const string Duration = "duration";
            public const string Result = "result";
            public const string ContainsProgress = "containsProgress";
            public const string ProgressTarget = "progressTarget";
            public const string ProgressCurrent = "progressCurrent";
            public const string ProgressTotal = "progressTotal";
            public const string ContainsFailures = "containsFailures";
            public const string Failures = "failures";
            public const string ContainsWarnings = "containsWarnings";
            public const string Warnings = "warnings";
            public const string ContainsInformation = "containsInformation";
            public const string Information = "information";
        }

        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public StatusReportViewModelDelegate(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public IDictionary<string, string> GetViewModel(IStatus status)
        {
            // viewmodel schemas
            var viewModel = new Dictionary<string, string>()
            {
                { statusReportViewModelSchema.Title, "" },
                { statusReportViewModelSchema.Complete, "" },
                { statusReportViewModelSchema.Started, "" },
                { statusReportViewModelSchema.Duration, "" },
                { statusReportViewModelSchema.Result, "" },
                { statusReportViewModelSchema.ContainsProgress, "" },
                { statusReportViewModelSchema.ProgressTarget, "" },
                { statusReportViewModelSchema.ProgressCurrent, "" },
                { statusReportViewModelSchema.ProgressTotal, "" },
                { statusReportViewModelSchema.ContainsFailures, ""},
                { statusReportViewModelSchema.Failures, ""},
                { statusReportViewModelSchema.ContainsWarnings, ""},
                { statusReportViewModelSchema.Warnings, ""},
                { statusReportViewModelSchema.ContainsInformation, ""},
                { statusReportViewModelSchema.Information, ""}
            };

            viewModel[statusReportViewModelSchema.Title] = status.Title;
            viewModel[statusReportViewModelSchema.Complete] = status.Complete ? "true" : "";
            viewModel[statusReportViewModelSchema.Started] = status.Started.ToLocalTime().ToString();
            viewModel[statusReportViewModelSchema.Duration] =
                status.Complete ?
                secondsFormattingController.Format((status.Completed - status.Started).Seconds) :
                "";

            var results = new List<string>();
            if (status.Failures != null && status.Failures.Any()) results.Add("Failure(s)");
            if (status.Warnings != null && status.Warnings.Any()) results.Add("Warning(s)");
            var result = string.Join(",", results);
            if (string.IsNullOrEmpty(result)) result = "Success";
            viewModel[statusReportViewModelSchema.Result] = result;

            if (status.Progress != null)
            {
                var current = status.Progress.Current;
                var total = status.Progress.Total;

                viewModel[statusReportViewModelSchema.ContainsProgress] = "true";
                viewModel[statusReportViewModelSchema.ProgressTarget] = status.Progress.Target;

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (status.Progress.Unit == DataUnits.Bytes)
                {
                    currentFormatted = bytesFormattingController.Format(current);
                    totalFormatted = bytesFormattingController.Format(total);
                }

                viewModel[statusReportViewModelSchema.ProgressCurrent] = currentFormatted;
                viewModel[statusReportViewModelSchema.ProgressTotal] = totalFormatted;
            }

            if (status.Failures != null && status.Failures.Any())
            {
                viewModel[statusReportViewModelSchema.ContainsFailures] = "true";
                viewModel[statusReportViewModelSchema.Failures] = string.Join("; ", status.Failures);
            }

            if (status.Warnings != null && status.Warnings.Any())
            {
                viewModel[statusReportViewModelSchema.ContainsWarnings] = "true";
                viewModel[statusReportViewModelSchema.Warnings] = string.Join("; ", status.Warnings);
            }

            if (status.Information != null && status.Information.Any())
            {
                viewModel[statusReportViewModelSchema.ContainsInformation] = "true";
                viewModel[statusReportViewModelSchema.Information] = string.Join("; ", status.Information);
            }

            return viewModel;
        }
    }
}
