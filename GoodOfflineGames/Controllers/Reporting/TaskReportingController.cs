using System;
using System.Collections.Generic;

using Interfaces.Console;
using Interfaces.Reporting;

namespace Controllers.Reporting
{
    public class TaskReportingController : ITaskReportingController
    {
        private Stack<string> names;
        private IConsoleController consoleController;

        private const string startReportTemplate = "Starting task [{0}]...";
        private const string completionReportTemplate = "Task [{0}] was completed successfully";
        private const string failureReportTemplate = "Task [{0}] failed with the following message: {1}";
        private const string warningReportTemplate = "Warning: {0}";
        private const string progressReportTemplate = "\r{0} out of {1} completed         ";
        private const string progressCompletedReportTemplate = "\r";

        public TaskReportingController(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            names = new Stack<string>();
        }

        public void StartTask(string name, params object[] values)
        {
            var formattedName = values == null ?
                name :
                string.Format(name, values);
            names.Push(formattedName);
            consoleController.WriteLine(startReportTemplate, MessageType.Progress, names.Peek());
        }

        public void CompleteTask()
        {
            consoleController.WriteLine(completionReportTemplate, MessageType.Success, names.Pop());
        }

        public void ReportFailure(string errorMessage)
        {
            consoleController.WriteLine(failureReportTemplate, MessageType.Error, names.Pop(), errorMessage);
        }

        public void ReportProgress(long value, long? maxValue, LongToStringFormattingDelegate formattingDelegate = null)
        {
            var formattedValue = formattingDelegate != null ?
                formattingDelegate(value) :
                value.ToString();

            var formattedMaxValue = formattingDelegate != null ?
                formattingDelegate((long)maxValue) :
                maxValue.ToString();

            if (value.Equals(maxValue)) consoleController.Write(progressCompletedReportTemplate, MessageType.Progress);
            else consoleController.Write(progressReportTemplate, MessageType.Progress, formattedValue, formattedMaxValue);
        }

        public void ReportWarning(string warningMessage)
        {
            consoleController.WriteLine(warningReportTemplate, MessageType.Warning, warningMessage);
        }
    }
}
