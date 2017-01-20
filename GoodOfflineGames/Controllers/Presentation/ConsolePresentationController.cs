using System;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Console;
using Interfaces.Measurement;
using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<Tuple<string, string[]>>
    {
        private IMeasurementController<string> formattedStringMeasurementController;
        private IConsoleController consoleController;

        private int[] logicalPreviousLengths;
        private int[] physicalPreviousLengths;

        private const int throttleMilliseconds = 250;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        public ConsolePresentationController(
            IMeasurementController<string> formattedStringMeasurementController,
            IConsoleController consoleController)
        {
            this.formattedStringMeasurementController = formattedStringMeasurementController;
            this.consoleController = consoleController;

            logicalPreviousLengths = new int[0];
            physicalPreviousLengths = new int[0];
        }

        private int PresentLine(int line, string content, string[] colors)
        {
            var currentLength = content.Length;
            var paddedContent = content;

            if (logicalPreviousLengths.Length > line)
                if (currentLength < logicalPreviousLengths[line])
                    paddedContent = content.PadRight(logicalPreviousLengths[line]);

            consoleController.WriteLine(paddedContent, colors);

            return currentLength;
        }

        public void Present(IEnumerable<Tuple<string, string[]>> viewModels, bool overrideThrottling = false)
        {
            if (!overrideThrottling && 
                (DateTime.UtcNow - lastReportedTimestamp).TotalMilliseconds < throttleMilliseconds) return;

            consoleController.SetCursorPosition(0, 0);

            var viewsModelsLength = viewModels.Count();
            var currentViewsLengths = new int[viewsModelsLength];

            for (var ii = 0; ii < viewsModelsLength; ii++)
                currentViewsLengths[ii] = 
                    PresentLine(
                        ii, 
                        viewModels.ElementAt(ii).Item1, 
                        viewModels.ElementAt(ii).Item2);

            if (logicalPreviousLengths.Length > viewsModelsLength)
                for (var ii = viewsModelsLength; ii < logicalPreviousLengths.Length; ii++)
                    consoleController.WriteLine(string.Empty.PadRight(logicalPreviousLengths[ii]));

            logicalPreviousLengths = currentViewsLengths;

            //consoleController.SetCursorPosition(0, 0);

            lastReportedTimestamp = DateTime.UtcNow;
        }
    }
}
