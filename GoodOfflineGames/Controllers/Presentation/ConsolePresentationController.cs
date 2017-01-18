using System;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Console;
using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<Tuple<string, string[]>>
    {
        private IConsoleController consoleController;
        private int[] previousViewsLengths;

        private const int throttleMilliseconds = 250;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        public ConsolePresentationController(
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            previousViewsLengths = new int[0];
        }

        private int PresentLine(int line, string content, string[] colors)
        {
            var currentLength = content.Length;
            var paddedContent = content;

            if (previousViewsLengths.Length > line)
                if (currentLength < previousViewsLengths[line])
                    paddedContent = content.PadRight(previousViewsLengths[line]);

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

            if (previousViewsLengths.Length > viewsModelsLength)
                for (var ii = viewsModelsLength; ii < previousViewsLengths.Length; ii++)
                    consoleController.WriteLine(string.Empty.PadRight(previousViewsLengths[ii]));

            previousViewsLengths = currentViewsLengths;

            //consoleController.SetCursorPosition(0, 0);

            lastReportedTimestamp = DateTime.UtcNow;
        }
    }
}
