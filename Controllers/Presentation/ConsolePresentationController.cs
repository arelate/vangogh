using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.LineBreaking;
using Interfaces.Console;
using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class ConsolePresentationController : IPresentationController<string[]>
    {
        private int previousFrameLines = 0;
        private int previousContentLines = 0;
        private StringBuilder stringFrameBuffer;

        private ILineBreakingDelegate lineBreakingDelegate;
        private IConsoleController consoleController;

        public ConsolePresentationController(
            ILineBreakingDelegate lineBreakingDelegate,
            IConsoleController consoleController)
        {
            stringFrameBuffer = new StringBuilder();

            this.lineBreakingDelegate = lineBreakingDelegate;
            this.consoleController = consoleController;
        }

        public void Present(params string[] lines)
        {
            stringFrameBuffer.Clear();

            // This assumes that all console presentation is controlled by the consolePresentationController.
            // Following previous frame that left cursor at the start of the new line after the output - move it back to 
            // (1,1) position relative to the previously outputed content. 
            // No need to worry about CursorLeft as it'll be on the 1st char
            if (previousFrameLines > 0)
                consoleController.CursorTop -= previousFrameLines;

            // Break lines with \n and wrap given available console window width
            var wrappedLines = lineBreakingDelegate.BreakLines(consoleController.WindowWidth, lines);

            // To build the buffer, we'll pad each line with space to take care of different line lengths
            foreach (var line in wrappedLines)
                stringFrameBuffer.Append(line.PadRight(consoleController.WindowWidth));

            // Also remove some extra lines that the previous content could have left
            var previousFrameOverflow = previousContentLines - wrappedLines.Count();
            if (previousFrameOverflow > 0)
                stringFrameBuffer.Append(
                    string.Empty.PadRight(consoleController.WindowWidth * previousFrameOverflow));

            // We use content lines to determine frame overflow (lines that won't be overwritten by current frame)
            // We use frame lines to determine how much we need to move cursor back
            previousContentLines = wrappedLines.Count();
            previousFrameLines = stringFrameBuffer.Length / consoleController.WindowWidth;

            // Finally, just write the current frame buffer, no need to worry about line breaks or new lines:
            // padded content length takes care of both
            consoleController.Write(stringFrameBuffer.ToString());
        }

        public Task PresentAsync(params string[] lines)
        {
            throw new NotImplementedException();
        }
    }
}
