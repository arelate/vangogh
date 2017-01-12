using System;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Console;
using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<string>
    {
        private IConsoleController consoleController;
        private int[] previousViewsLengths;

        public ConsolePresentationController(
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            previousViewsLengths = new int[0];
        }

        public void Present(IEnumerable<string> views)
        {
            consoleController.SetCursorPosition(0, 0);

            var viewsLength = views.Count();

            var currentViewsLength = new int[viewsLength];
            for (var ii = 0; ii < viewsLength; ii++)
            {
                currentViewsLength[ii] = views.ElementAt(ii).Length;
            }

            for (var ii = 0; ii < viewsLength; ii++)
            {
                var view = views.ElementAt(ii);

                if (ii < previousViewsLengths.Length)
                    view = view.PadRight(previousViewsLengths[ii]);

                consoleController.WriteLine(view);
            }

            if (previousViewsLengths.Length > currentViewsLength.Length)
                for (var ii = currentViewsLength.Length; ii < previousViewsLengths.Length; ii++)
                    consoleController.WriteLine(string.Empty.PadRight(previousViewsLengths[ii]));

            previousViewsLengths = currentViewsLength;

            consoleController.SetCursorPosition(0, 0);
        }
    }
}
