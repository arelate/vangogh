using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.LineBreaking;
using Interfaces.Measurement;

using Models.Separators;

namespace Controllers.LineBreaking
{
    // TODO: This implements naive line breaking assuming every character is break opportunity
    // This behavior aligns with how Console breaks text.
    // In the future consider changing this to use spaces or punctuation as a break opportunities
    public class LineBreakingController : ILineBreakingController
    {
        private IMeasurementController<string> formattedStringMeasurementController;

        public LineBreakingController(IMeasurementController<string> formattedStringMeasurementController)
        {
            this.formattedStringMeasurementController = formattedStringMeasurementController;
        }

        public string[] BreakLines(string content, int availableWidth)
        {
            if (formattedStringMeasurementController.Measure(content) <= availableWidth)
                return new string[] { content };

            var contentLines = new List<string>();
            var contentParts = content.Split(new string[] { Separators.ColorFormatting }, StringSplitOptions.None);

            var currentLine = new StringBuilder(availableWidth);
            var counter = 0;

            for (var ii=0; ii<contentParts.Length; ii++)
            {
                foreach (var letter in contentParts[ii])
                {
                    if (++counter > availableWidth)
                    {
                        contentLines.Add(currentLine.ToString());
                        currentLine.Clear();
                        counter = 0;
                    }

                    currentLine.Append(letter);
                }

                // don't add extra color formatting break for the last element
                if (ii == contentParts.Length - 1) break;

                currentLine.Append(Separators.ColorFormatting);
            }

            if (currentLine.Length > 0)
                contentLines.Add(currentLine.ToString());

            return contentLines.ToArray();
        }
    }
}
