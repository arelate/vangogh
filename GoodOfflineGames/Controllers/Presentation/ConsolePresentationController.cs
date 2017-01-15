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

        public ConsolePresentationController(
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;
            previousViewsLengths = new int[0];
        }

        public void Present(IEnumerable<Tuple<string, string[]>> viewModels)
        {
            consoleController.SetCursorPosition(0, 0);

            var viewsModelsLength = viewModels.Count();
            var currentViewsLengths = new int[viewsModelsLength];

            for (var ii = 0; ii < viewsModelsLength; ii++)
                currentViewsLengths[ii] = viewModels.ElementAt(ii).Item1.Length;

            var viewLengthChanged = currentViewsLengths.Length != previousViewsLengths.Length;

            if (!viewLengthChanged)
            {
                for (var ii = 0; ii < viewsModelsLength; ii++)
                    viewLengthChanged |= currentViewsLengths[ii] != previousViewsLengths[ii];
            }

            if (viewLengthChanged)
            {
                consoleController.Write(string.Empty.PadLeft(System.Console.WindowWidth * System.Console.WindowHeight, ' '));
                consoleController.SetCursorPosition(0, 0);
            }

            for (var ii = 0; ii < viewsModelsLength; ii++)
            {
                var viewModelText = viewModels.ElementAt(ii).Item1;
                var viewModelColors = viewModels.ElementAt(ii).Item2;

                consoleController.WriteLine(viewModelText, viewModelColors);
            }

            previousViewsLengths = currentViewsLengths;

            consoleController.SetCursorPosition(0, 0);
        }
    }
}
