using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.LineBreaking;

using Models.Separators;

namespace Controllers.LineBreaking
{
    public class LineBreakingDelegate : ILineBreakingDelegate
    {
        public IEnumerable<string> BreakLines(int availableWidth, IEnumerable<string> lines)
        {
            var brokenLines = new List<string>();

            foreach (var line in lines)
                foreach (var splitLine in line.Split(Separators.Common.NewLine, StringSplitOptions.None))
                {
                    var segment = splitLine;

                    do
                    {
                        var needToBreak = segment.Length > availableWidth;

                        brokenLines.Add(needToBreak ?
                            segment.Substring(0, availableWidth) :
                            segment);
                        segment = needToBreak ? 
                            segment.Substring(availableWidth, segment.Length - availableWidth) : 
                            string.Empty;

                    } while (segment.Length >= availableWidth);

                    if (!string.IsNullOrEmpty(segment))
                        brokenLines.Add(segment);
                }

            return brokenLines;
        }
    }
}
