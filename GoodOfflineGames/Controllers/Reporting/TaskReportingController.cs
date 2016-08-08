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
        private const string completionReportTemplate = "Task [{0}] was completed successfully.";
        private const string failureReportTemplate = "Task [{0}] failed with the following message: {1}";
        private const string progressReportTemplate = "\r{0} out of {1} completed.          ";

        public TaskReportingController(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            names = new Stack<string>();
        }

        public void AddTask(string name)
        {
            names.Push(name);
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

        public void ReportProgress(long value, long maxValue)
        {
            if (value == maxValue) consoleController.WriteLine(progressReportTemplate, MessageType.Progress, value, maxValue);
            else consoleController.Write(progressReportTemplate, MessageType.Progress, value, maxValue);
        }
    }
}
