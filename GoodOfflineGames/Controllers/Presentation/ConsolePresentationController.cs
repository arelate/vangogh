using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Console;
using Interfaces.LineBreaking;
using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<string>
    {
        private ILineBreakingController lineBreakingController;
        private IConsoleController consoleController;

        private IList<int> previousScreenLinesLengths;

        private const int clearIterationsCount = 100; // clear console every N presentations
        private int presentationIteration = 0;

        public ConsolePresentationController(
            ILineBreakingController lineBreakingController,
            IConsoleController consoleController)
        {
            this.lineBreakingController = lineBreakingController;
            this.consoleController = consoleController;

            consoleController.Clear();

            previousScreenLinesLengths = new List<int>();
            consoleController.CursorVisible = false;
        }

        private int PresentLine(int line, string content)
        {
            var currentLength = content.Length;
            var previousLineLength = previousScreenLinesLengths.ElementAtOrDefault(line);
            var paddedContent = content;

            consoleController.SetCursorPosition(0, line);

            if (previousLineLength > currentLength)
                paddedContent = content.PadRight(content.Length + previousLineLength - currentLength);

            consoleController.Write(paddedContent);

            return currentLength;
        }

        private void PresentViewModel(string text, ref int currentScreenLine, IList<int> currentLinesLengths)
        {
            var lines = lineBreakingController.BreakLines(text, consoleController.WindowWidth);

            foreach (var line in lines)
            {
                var currentLineLength = PresentLine(
                        currentScreenLine++,
                        line);

                currentLinesLengths.Add(currentLineLength);
            }
        }

        public void Present(IEnumerable<string> views)
        {
            if (views.Count() == 0) return;

            if (++presentationIteration == clearIterationsCount)
            {
                previousScreenLinesLengths.Clear();
                consoleController.Clear();
                presentationIteration = 0;
            }

            var wrappedViews = new List<string>();
            foreach (var view in views)
                wrappedViews.AddRange(view.Split(new string[] { "\n" }, StringSplitOptions.None));

            var currentLinesLengths = new List<int>();
            var currentScreenLine = 0;

            foreach (var view in wrappedViews)
                PresentViewModel(
                    view, 
                    ref currentScreenLine, 
                    currentLinesLengths);

            // erase previous lines that might be left on the screen
            var previousLinesCount = previousScreenLinesLengths.Count();
            if (previousLinesCount > 0 &&
                previousLinesCount >= currentScreenLine)
            {
                for (var pp = currentScreenLine; pp < previousLinesCount; pp++)
                {
                    consoleController.SetCursorPosition(0, currentScreenLine++);
                    consoleController.Write(string.Empty.PadRight(previousScreenLinesLengths[pp]));
                }
            }

            previousScreenLinesLengths = currentLinesLengths;
        }

        public Task PresentAsync(IEnumerable<string> views)
        {
            throw new NotImplementedException();
        }
    }
}
