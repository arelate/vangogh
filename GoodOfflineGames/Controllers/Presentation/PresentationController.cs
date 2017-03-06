using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;

using Interfaces.Console;
using Interfaces.Measurement;
using Interfaces.LineBreaking;
using Interfaces.Presentation;

using Models.Separators;

namespace Controllers.Presentation
{
    public class PresentationController : IPresentationController<Tuple<string, string[]>>
    {
        private IMeasurementController<string> formattedStringMeasurementController;
        private ILineBreakingController lineBreakingController;
        private IConsoleController consoleController;

        private IList<int> previousScreenLinesLengths;

        private const int throttleMilliseconds = 200;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        private int previousWindowWidth;
        private int previousWindowHeight;

        public PresentationController(
            IMeasurementController<string> formattedStringMeasurementController,
            ILineBreakingController lineBreakingController,
            IConsoleController consoleController)
        {
            this.formattedStringMeasurementController = formattedStringMeasurementController;
            this.lineBreakingController = lineBreakingController;
            this.consoleController = consoleController;

            previousScreenLinesLengths = new List<int>();
            consoleController.CursorVisible = false;

            previousWindowWidth = consoleController.WindowWidth;
            previousWindowHeight = consoleController.WindowHeight;
        }

        private int PresentLine(int line, string content, string[] colors)
        {
            var currentLength = formattedStringMeasurementController.Measure(content);
            var previousLineLength = previousScreenLinesLengths.ElementAtOrDefault(line);
            var paddedContent = content;

            consoleController.SetCursorPosition(0, line);

            if (previousLineLength > currentLength)
                paddedContent = content.PadRight(content.Length + previousLineLength - currentLength);

            consoleController.Write(paddedContent, colors);

            return currentLength;
        }

        private void PresentViewModel(string text, string[] colors, ref int currentScreenLine, IList<int> currentLinesLengths)
        {
            var lines = lineBreakingController.BreakLines(text, consoleController.WindowWidth);
            var consumedColors = 0;

            foreach (var line in lines)
            {
                var requiredColors = Regex.Matches(line, Separators.ColorFormatting).Count;
                var lineColors = new List<string>();
                for (var cc = consumedColors; cc < consumedColors + requiredColors; cc++)
                    lineColors.Add(colors.ElementAtOrDefault(cc));

                consumedColors += requiredColors;

                var currentLineLength = PresentLine(
                        currentScreenLine++,
                        line,
                        lineColors.ToArray());

                currentLinesLengths.Add(currentLineLength);
            }

            consoleController.ResetFormatting();
        }

        public void Present(IEnumerable<Tuple<string, string[]>> viewModels, bool overrideThrottling = false)
        {
            if (!overrideThrottling && 
                (DateTime.UtcNow - lastReportedTimestamp).TotalMilliseconds < throttleMilliseconds) return;

            if (previousWindowWidth != consoleController.WindowWidth ||
                previousWindowHeight != consoleController.WindowHeight)
                consoleController.Clear();

            var viewsModelsLength = viewModels.Count();
            var currentLinesLengths = new List<int>();
            var currentScreenLine = 0;

            for (var ii = 0; ii < viewsModelsLength; ii++)
            {
                var text = viewModels.ElementAt(ii).Item1;
                var colors = viewModels.ElementAt(ii).Item2;

                PresentViewModel(
                    text, 
                    colors, 
                    ref currentScreenLine, 
                    currentLinesLengths);
            }

            var previousLinesCount = previousScreenLinesLengths.Count();
            if (previousLinesCount >= currentScreenLine)
            {
                for (var pp = currentScreenLine; pp < previousLinesCount; pp++)
                {
                    consoleController.SetCursorPosition(0, currentScreenLine++);
                    consoleController.Write(string.Empty.PadRight(consoleController.WindowWidth));
                }
            }

            previousScreenLinesLengths = currentLinesLengths;

            previousWindowWidth = consoleController.WindowWidth;
            previousWindowHeight = consoleController.WindowHeight;

            lastReportedTimestamp = DateTime.UtcNow;
        }
    }
}
