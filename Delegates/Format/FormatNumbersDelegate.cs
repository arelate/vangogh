using System;
using System.Linq;

using Interfaces.Delegates.Format;

namespace Delegates.Format
{
    public abstract class FormatNumbersDelegate : IFormatDelegate<long, string>
    {
        protected long[] relativeOrders;
        protected string[] orderTitles;
        protected string format;
        protected string zero;
        protected bool roundValue = false;

        public string Format(long value)
        {
            long max = relativeOrders.Aggregate((long)1, (a, b) => a * b);

            for (var ii = 0; ii < relativeOrders.Length; ii++)
            {
                if (value >= max)
                {
                    var outputValue = decimal.Divide(value, max);
                    if (roundValue) outputValue = Math.Round(outputValue);
                    return string.Format(
                        format,
                        outputValue,
                        orderTitles[ii]);
                }

                max /= relativeOrders[ii];
            }
            return zero;
        }
    }
}
