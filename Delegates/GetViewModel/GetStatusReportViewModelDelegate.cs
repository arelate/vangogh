using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Format;

using Interfaces.ViewModel;
using Interfaces.Status;

using Models.Units;

namespace Delegates.GetViewModel
{
    // TODO: Convert delegate
    public class GetStatusReportViewModelDelegate : IGetViewModelDelegate<IStatus>
    {
        static class StatusReportViewModelSchema
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

        readonly IFormatDelegate<long, string> formatBytesDelegate;
        readonly IFormatDelegate<long, string> formatSecondsDelegate;

        public GetStatusReportViewModelDelegate(
            IFormatDelegate<long, string> formatBytesDelegate,
            IFormatDelegate<long, string> formatSecondsDelegate)
        {
            this.formatBytesDelegate = formatBytesDelegate;
            this.formatSecondsDelegate = formatSecondsDelegate;
        }

        public IDictionary<string, string> GetViewModel(IStatus status)
        {
            // viewmodel schemas
            var viewModel = new Dictionary<string, string>
            {
                { StatusReportViewModelSchema.Title, "" },
                { StatusReportViewModelSchema.Complete, "" },
                { StatusReportViewModelSchema.Started, "" },
                { StatusReportViewModelSchema.Duration, "" },
                { StatusReportViewModelSchema.Result, "" },
                { StatusReportViewModelSchema.ContainsProgress, "" },
                { StatusReportViewModelSchema.ProgressTarget, "" },
                { StatusReportViewModelSchema.ProgressCurrent, "" },
                { StatusReportViewModelSchema.ProgressTotal, "" },
                { StatusReportViewModelSchema.ContainsFailures, ""},
                { StatusReportViewModelSchema.Failures, ""},
                { StatusReportViewModelSchema.ContainsWarnings, ""},
                { StatusReportViewModelSchema.Warnings, ""},
                { StatusReportViewModelSchema.ContainsInformation, ""},
                { StatusReportViewModelSchema.Information, ""}
            };

            viewModel[StatusReportViewModelSchema.Title] = status.Title;
            viewModel[StatusReportViewModelSchema.Complete] = status.Complete ? "true" : "";
            viewModel[StatusReportViewModelSchema.Started] = status.Started.ToLocalTime().ToString();
            viewModel[StatusReportViewModelSchema.Duration] =
                status.Complete ?
                formatSecondsDelegate.Format((status.Completed - status.Started).Seconds) :
                "";

            var results = new List<string>();
            if (status.Failures != null && status.Failures.Any()) results.Add("Failure(s)");
            if (status.Warnings != null && status.Warnings.Any()) results.Add("Warning(s)");
            var result = string.Join(",", results);
            if (string.IsNullOrEmpty(result)) result = "Success";
            viewModel[StatusReportViewModelSchema.Result] = result;

            if (status.Progress != null)
            {
                var current = status.Progress.Current;
                var total = status.Progress.Total;

                viewModel[StatusReportViewModelSchema.ContainsProgress] = "true";
                viewModel[StatusReportViewModelSchema.ProgressTarget] = status.Progress.Target;

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (status.Progress.Unit == DataUnits.Bytes)
                {
                    currentFormatted = formatBytesDelegate.Format(current);
                    totalFormatted = formatBytesDelegate.Format(total);
                }

                viewModel[StatusReportViewModelSchema.ProgressCurrent] = currentFormatted;
                viewModel[StatusReportViewModelSchema.ProgressTotal] = totalFormatted;
            }

            if (status.Failures != null && status.Failures.Any())
            {
                viewModel[StatusReportViewModelSchema.ContainsFailures] = "true";
                viewModel[StatusReportViewModelSchema.Failures] = string.Join("; ", status.Failures);
            }

            if (status.Warnings != null && status.Warnings.Any())
            {
                viewModel[StatusReportViewModelSchema.ContainsWarnings] = "true";
                viewModel[StatusReportViewModelSchema.Warnings] = string.Join("; ", status.Warnings);
            }

            if (status.Information != null && status.Information.Any())
            {
                viewModel[StatusReportViewModelSchema.ContainsInformation] = "true";
                viewModel[StatusReportViewModelSchema.Information] = string.Join("; ", status.Information);
            }

            return viewModel;
        }
    }
}
