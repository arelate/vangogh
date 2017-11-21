using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.LineBreaking;
using Interfaces.Console;
using Interfaces.RequestPresent;

namespace Controllers.RequestPresent
{
    public class ConsoleRequestPresentController : IRequestPresentController<string>
    {
        private int savedCursorTopPosition = -1;

        private int remainingPreviousFrameLines = 0;

        private StringBuilder fragmentBuffer;

        private ILineBreakingDelegate lineBreakingDelegate;
        private IConsoleController consoleController;

        public ConsoleRequestPresentController(
            ILineBreakingDelegate lineBreakingDelegate,
            IConsoleController consoleController)
        {
            fragmentBuffer = new StringBuilder();

            this.lineBreakingDelegate = lineBreakingDelegate;
            this.consoleController = consoleController;
        }

        public void PresentAdditional(params string[] data)
        {
            //    fragmentBuffer.Clear();

            //    // Break lines with \n and wrap given available console window width
            //    var fragmentWrappedLines = lineBreakingDelegate.BreakLines(consoleController.WindowWidth, lines);

            //    // To build the buffer, we'll pad each line with space to take care of different line lengths
            //    foreach (var line in fragmentWrappedLines)
            //        fragmentBuffer.Append(line.PadRight(consoleController.WindowWidth));

            //    // Also remove some extra lines that the previous frame could have left
            //    var previousFrameOverflow = remainingPreviousFrameLines - fragmentWrappedLines.Count();
            //    if (previousFrameOverflow > 0)
            //    {
            //        remainingPreviousFrameLines -= fragmentWrappedLines.Count();
            //        fragmentBuffer.Append(
            //            string.Empty.PadRight(consoleController.WindowWidth * previousFrameOverflow));
            //    }

            //    //// We use content lines to determine frame overflow (lines that won't be overwritten by current frame)
            //    //// We use frame lines to determine how much we need to move cursor back
            //    //previousFragmentWrappedLines = fragmentWrappedLines.Count();
            //    //previousFrameLines += fragmentBuffer.Length / consoleController.WindowWidth;

            //    // Finally, just write the current frame buffer, no need to worry about line breaks or new lines:
            //    // padded content length takes care of both
            //    consoleController.Write(fragmentBuffer.ToString());

            //    if (previousFrameOverflow > 0)
            //    {
            //        consoleController.CursorTop -= previousFrameOverflow;
            //    }

            throw new NotImplementedException();
        }

        public void PresentNew(params string[] data)
        {
            //    // This assumes that all console presentation is controlled by the consolePresentationController.
            //    // Following previous frame that left cursor at the start of the new line after the output - move it back to 
            //    // (1,1) position relative to the previously outputed content. 
            //    // No need to worry about CursorLeft as it'll be on the 1st char
            //    //var previousFrameLines = previousFrameContentLines + currentFramePaddingLines;
            //    //if (previousFrameLines > 0)
            //    //{
            //    //    consoleController.CursorTop -= previousFrameLines;
            //    //    previousFragmentWrappedLines = previousFrameLines;
            //    //    previousFrameContentLines = 0;
            //    //}

            //    if (savedCursorTopPosition > -1)
            //    {
            //        remainingPreviousFrameLines = consoleController.CursorTop - savedCursorTopPosition;
            //        consoleController.CursorTop = savedCursorTopPosition;
            //        consoleController.CursorLeft = 0;
            //    }

            // this.PresentAdditional(data);

            throw new NotImplementedException();
        }

        public void PresentSticky(params string[] data)
        {
            throw new NotImplementedException();
        }

        public string RequestData(string message)
        {
            throw new NotImplementedException();
        }

        public string RequestPrivateData(string message)
        {
            throw new NotImplementedException();
        }
    }
}
