using System;
using System.Linq;
using System.Text;

using Interfaces.LineBreaking;
using Interfaces.Console;
using Interfaces.Output;
using Interfaces.Input;

namespace Controllers.InputOutput
{
    public class ConsoleInputOutputController :
        IInputController<string>,
        IOutputController<string[]>
    {
        private int savedCursorTopPosition = -1;
        private int previousFrameLines = 0;

        private StringBuilder fragmentBuffer;

        private ILineBreakingDelegate lineBreakingDelegate;
        private IConsoleController consoleController;

        public ConsoleInputOutputController(
            ILineBreakingDelegate lineBreakingDelegate,
            IConsoleController consoleController)
        {
            fragmentBuffer = new StringBuilder();

            this.lineBreakingDelegate = lineBreakingDelegate;
            this.consoleController = consoleController;
        }

        public void SetRefresh()
        {
            if (savedCursorTopPosition < 0) return;
            // Save current (soon to be previous) frame lines count. 
            // That will be used in the next frame to clear any remaning outputted lines
            previousFrameLines = consoleController.CursorTop - savedCursorTopPosition;
            // Reset/refresh cursor position to initial state
            consoleController.CursorTop = savedCursorTopPosition;
            // Make sure it's set to the start of the line for console providers with zero cursor width
            consoleController.CursorLeft = 0;
        }

        public void ClearContinuousLines(int lines)
        {
            if (lines < 1) return;
            // Erase remaining previous frame lines with the padded space
            consoleController.Write(
                 string.Empty.PadRight(consoleController.WindowWidth * lines));
            // Move cursor back so that next additional presentation will continue from the actual content
            consoleController.CursorTop -= lines;
            // This sets the correct state - we no longer need to track previous frame lines
            previousFrameLines = 0;
        }

        public void OutputContinuous(params string[] data)
        {
            // Clear frame buffer
            fragmentBuffer.Clear();
            // Break the lines with new line separator, also wrap lines given the available console width
            var fragmentWrappedLines = lineBreakingDelegate.BreakLines(consoleController.WindowWidth, data);
            // To build the buffer, we'll pad each line with spaces.
            // That takes care of previous frame lines that could have been longer
            foreach (var line in fragmentWrappedLines)
                fragmentBuffer.Append(line.PadRight(consoleController.WindowWidth));
            // Write the current frame buffer, no need to worry about line breaks or new lines:
            // padded content length takes care of both
            consoleController.Write(fragmentBuffer.ToString());
            // For consoles that have zero cursor width make sure to forcefully break the line
            if (consoleController.CursorLeft > 0)
                consoleController.WriteLine(string.Empty);
            // Clear remaining lines from the previous frame, if any
            ClearContinuousLines(previousFrameLines - fragmentWrappedLines.Count());
        }

        public void OutputOnRefresh(params string[] data)
        {
            // Refresh the frame cursor to the initial position
            SetRefresh();
            // Preserve the current position
            savedCursorTopPosition = consoleController.CursorTop;
            // Use regular continuous output
            OutputContinuous(data);
        }

        public void OutputFixedOnRefresh(params string[] data)
        {
            // Fixed output is a combination of output on refresh and ...
            OutputOnRefresh(data);
            // ...saving new cursor position after fixed content, so that new content always goes below fixed
            savedCursorTopPosition = consoleController.CursorTop;
        }

        public string RequestInput(string message)
        {
            OutputContinuous(message);
            return consoleController.ReadLine();
        }

        public string RequestPrivateInput(string message)
        {
            OutputContinuous(message);
            return consoleController.ReadLinePrivate();
        }
    }
}
