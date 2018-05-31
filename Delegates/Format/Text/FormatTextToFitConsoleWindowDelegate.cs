using System;
using System.Collections.Generic;

using Interfaces.Delegates.Format;

using Interfaces.Controllers.Console;

using Models.Separators;

namespace Delegates.Format.Text
{
    public class FormatTextToFitConsoleWindowDelegate : IFormatDelegate<IEnumerable<string>, IEnumerable<string>>
    {
        readonly IWindowWidthProperty windowWidthProperty;

        public FormatTextToFitConsoleWindowDelegate(IWindowWidthProperty windowWidthProperty)
        {
            this.windowWidthProperty = windowWidthProperty;
        }

        public IEnumerable<string> Format(IEnumerable<string> lines)
        {
            var brokenLines = new List<string>();
            var availableWidth = (windowWidthProperty != null) ?
                windowWidthProperty.WindowWidth :
                40;

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
