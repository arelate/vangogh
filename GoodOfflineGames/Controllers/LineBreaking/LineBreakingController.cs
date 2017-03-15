using System;

using Interfaces.LineBreaking;

namespace Controllers.LineBreaking
{
    public class LineBreakingController : ILineBreakingController
    {
        public string[] BreakLines(string content, int availableWidth)
        {
            if (content.Length <= availableWidth)
                return new string[] { content };

            var linesCount = (content.Length / availableWidth) + 1;
            var contentLines = new string[linesCount];
            var remainingWidth = content.Length;

            for (var ii = 0; ii < linesCount; ii++)
            {
                contentLines[ii] = content.Substring(ii * availableWidth, Math.Min(availableWidth, remainingWidth));
                remainingWidth -= availableWidth;
            }

            return contentLines;
        }
    }
}
