using System.Linq;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public abstract class FormattingController : IFormattingController
    {
        protected long[] relativeOrders;
        protected string[] orderTitles;
        protected string format;
        protected string zero;

        public string Format(long value)
        {
            long max = relativeOrders.Aggregate((long)1, (a, b) => a * b);

            for (var ii = 0; ii < relativeOrders.Length; ii++)
            {
                if (value >= max)
                    return string.Format(
                        format, 
                        decimal.Divide(value, max), 
                        orderTitles[ii]);

                max /= relativeOrders[ii];
            }
            return zero;
        }
    }
}
