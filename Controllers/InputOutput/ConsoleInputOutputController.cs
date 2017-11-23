using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (savedCursorTopPosition > -1)
            {
                previousFrameLines = consoleController.CursorTop - savedCursorTopPosition;
                consoleController.CursorTop = savedCursorTopPosition;
                consoleController.CursorLeft = 0;
            }
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
            fragmentBuffer.Clear();

            // Break lines with \n and wrap given available console window width
            var fragmentWrappedLines = lineBreakingDelegate.BreakLines(consoleController.WindowWidth, data);

            // To build the buffer, we'll pad each line with spaces 
            // to take care of previous frame lines that could have been longer
            foreach (var line in fragmentWrappedLines)
                fragmentBuffer.Append(line.PadRight(consoleController.WindowWidth));

            // Write the current frame buffer, no need to worry about line breaks or new lines:
            // padded content length takes care of both
            consoleController.Write(fragmentBuffer.ToString());

            // clear lines remaining from the previous frame, if any
            ClearContinuousLines(previousFrameLines - fragmentWrappedLines.Count());
        }

        public void OutputOnRefresh(params string[] data)
        {
            // Set the cursor to the initial position
            SetRefresh();

            // preserve the current position
            savedCursorTopPosition = consoleController.CursorTop;

            //
            OutputContinuous(data);
        }

        public void OutputFixed(params string[] data)
        {
            throw new NotImplementedException();
        }

        public string RequestInput(string message)
        {
            throw new NotImplementedException();
        }

        public string RequestPrivateInput(string message)
        {
            throw new NotImplementedException();
        }
    }
}
