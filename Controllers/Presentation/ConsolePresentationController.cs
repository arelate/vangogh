using System;
using System.Threading.Tasks;

using Interfaces.Console;
using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<string>
    {
        private int lastLinesCount = int.MinValue;

        private IConsoleController consoleController;

        public ConsolePresentationController(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public void Present(string singleLineView)
        {
            if (string.IsNullOrEmpty(singleLineView)) return;

            if (lastLinesCount != int.MinValue)
            {
                consoleController.CursorLeft = 0;
                consoleController.CursorTop -= lastLinesCount;
            }

            var lines = (singleLineView.Length / consoleController.WindowWidth) + 1;

            consoleController.Write(singleLineView.PadRight(lines * consoleController.WindowWidth));

            if (lastLinesCount > lines)
            {
                for (var ii = lines; ii < lastLinesCount; ii++)
                    consoleController.Write("".PadRight(consoleController.WindowWidth));
                consoleController.CursorTop -= lastLinesCount - lines;
            }

            lastLinesCount = lines;
        }

        public Task PresentAsync(string views)
        {
            throw new NotImplementedException();
        }
    }
}
