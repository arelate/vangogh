using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.Format;

using Interfaces.Controllers.Output;
using Interfaces.Controllers.Console;

using Interfaces.Input;

using Interfaces.Status;

namespace Controllers.InputOutput
{
    public class ConsoleInputOutputController :
        IInputController<string>,
        IOutputController<string[]>
    {
        int savedCursorTopPosition = -1;
        int previousFrameLines;

        StringBuilder fragmentBuffer;

        IFormatDelegate<IEnumerable<string>, IEnumerable<string>> formatTextToFitConsoleWindowDelegate;
        readonly IConsoleController consoleController;

        public ConsoleInputOutputController(
            IFormatDelegate<IEnumerable<string>, IEnumerable<string>> formatTextToFitConsoleWindowDelegate,
            IConsoleController consoleController)
        {
            fragmentBuffer = new StringBuilder();

            this.formatTextToFitConsoleWindowDelegate = formatTextToFitConsoleWindowDelegate;
            this.consoleController = consoleController;
        }

        public async Task SetRefreshAsync()
        {
            await Task.Run(() =>
            {
                if (savedCursorTopPosition < 0) return;
                // Save current (soon to be previous) frame lines count. 
                // That will be used in the next frame to clear any remaning outputted lines
                previousFrameLines = consoleController.CursorTop - savedCursorTopPosition;
                // Reset/refresh cursor position to initial state
                consoleController.CursorTop = savedCursorTopPosition;
                // Make sure it's set to the start of the line for console providers with zero cursor width
                consoleController.CursorLeft = 0;
            });
        }

        public async Task ClearContinuousLinesAsync(int lines)
        {
            await Task.Run(() =>
            {
                if (lines < 1) return;
                // Erase remaining previous frame lines with the padded space
                consoleController.Write(
                     string.Empty.PadRight(consoleController.WindowWidth * lines));
                // Move cursor back so that next additional presentation will continue from the actual content
                consoleController.CursorTop -= lines;
                // This sets the correct state - we no longer need to track previous frame lines
                previousFrameLines = 0;
            });
        }

        public async Task OutputContinuousAsync(IStatus status, params string[] data)
        {
            // Clear frame buffer
            fragmentBuffer.Clear();
            // Break the lines with new line separator, also wrap lines given the available console width
            var fragmentWrappedLines = formatTextToFitConsoleWindowDelegate.Format(data);
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
            await ClearContinuousLinesAsync(previousFrameLines - fragmentWrappedLines.Count());
        }

        public async Task OutputOnRefreshAsync(params string[] data)
        {
            // Refresh the frame cursor to the initial position
            await SetRefreshAsync();
            // Preserve the current position
            savedCursorTopPosition = consoleController.CursorTop;
            // Use regular continuous output
            await OutputContinuousAsync(null, data);
        }

        public async Task OutputFixedOnRefreshAsync(params string[] data)
        {
            // Fixed output is a combination of output on refresh and ...
            await OutputOnRefreshAsync(data);
            // ...saving new cursor position after fixed content, so that new content always goes below fixed
            savedCursorTopPosition = consoleController.CursorTop;
        }

        public async Task<string> RequestInputAsync(string message)
        {
            await OutputContinuousAsync(null, message);
            return consoleController.ReadLine();
        }

        public async Task<string> RequestPrivateInputAsync(string message)
        {
            await OutputContinuousAsync(null, message);
            return consoleController.ReadLinePrivate();
        }
    }
}
