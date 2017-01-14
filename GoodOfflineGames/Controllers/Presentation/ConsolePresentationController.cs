using System;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Console;
using Interfaces.Presentation;

using Models.ViewModels;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<Tuple<string, string[]>>
    {
        private IConsoleController consoleController;
        private int[] previousViewsLengths;

        public ConsolePresentationController(
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            previousViewsLengths = new int[0];
        }

        public void Present(IEnumerable<Tuple<string, string[]>> views)
        {
            consoleController.SetCursorPosition(0, 0);

            var viewsLength = views.Count();

            var currentViewsLength = new int[viewsLength];
            for (var ii = 0; ii < viewsLength; ii++)
            {
                var viewText = views.ElementAt(ii).Item1;
                currentViewsLength[ii] = viewText.Length;
            }

            for (var ii = 0; ii < viewsLength; ii++)
            {
                var viewText = views.ElementAt(ii).Item1;
                var viewColors = views.ElementAt(ii).Item2;

                if (ii < previousViewsLengths.Length)
                    viewText = viewText.PadRight(previousViewsLengths[ii]);

                consoleController.WriteLine(viewText, viewColors);
            }

            if (previousViewsLengths.Length > currentViewsLength.Length)
                for (var ii = currentViewsLength.Length; ii < previousViewsLengths.Length; ii++)
                    consoleController.WriteLine(string.Empty.PadRight(previousViewsLengths[ii]));

            previousViewsLengths = currentViewsLength;

            consoleController.SetCursorPosition(0, 0);
        }
    }
}
