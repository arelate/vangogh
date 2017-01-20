using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Interfaces.Measurement;

using Models.Separators;

namespace Controllers.Measurement
{
    public class FormattedStringMeasurementController : IMeasurementController<string>
    {
        public long Measure(string data)
        {
            return data.Length - Regex.Matches(data, Separators.ConsoleColor).Count * Separators.ConsoleColor.Length;
        }
    }
}
