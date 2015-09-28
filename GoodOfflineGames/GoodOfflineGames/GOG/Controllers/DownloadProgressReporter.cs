using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public class DownloadProgressReporter : IProgress<double>
    {
        private IConsoleController consoleController;
        private string lastReportedValue;

        public DownloadProgressReporter(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public void Report(double value)
        {
            string formattedValue = string.Format("\r{0:P1}...", value);
            if (formattedValue != lastReportedValue)
            {
                lastReportedValue = formattedValue;
                consoleController.Write(formattedValue);
            }
        }
    }

}
