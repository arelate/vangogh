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

        private bool presenting;

        public ConsolePresentationController(
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            previousViewsLengths = new int[0];
        }

        public void Present(IEnumerable<Tuple<string, string[]>> viewModels)
        {
            if (presenting)
            {
                consoleController.DefaultColor = ConsoleColor.Red;
            }

            presenting = true;

            consoleController.SetCursorPosition(0, 0);

            var viewsModelsLength = viewModels.Count();

            var currentViewsLength = new int[viewsModelsLength];
            for (var ii = 0; ii < viewsModelsLength; ii++)
            {
                var viewModelText = viewModels.ElementAt(ii).Item1;
                var viewModelColors = viewModels.ElementAt(ii).Item2;

                currentViewsLength[ii] = viewModelText.Length;

                if (ii < previousViewsLengths.Length)
                    viewModelText = viewModelText.PadRight(previousViewsLengths[ii]);

                consoleController.WriteLine(viewModelText, viewModelColors);
            }

            if (previousViewsLengths.Length > currentViewsLength.Length)
                for (var ii = currentViewsLength.Length; ii < previousViewsLengths.Length; ii++)
                    consoleController.WriteLine(string.Empty.PadRight(previousViewsLengths[ii]));

            previousViewsLengths = currentViewsLength;

            consoleController.SetCursorPosition(0, 0);

            presenting = false;
        }
    }
}
