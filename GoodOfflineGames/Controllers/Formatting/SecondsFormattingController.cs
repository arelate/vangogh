using Models.Units;

namespace Controllers.Formatting
{
    public class SecondsFormattingController : FormattingController
    {
        public SecondsFormattingController()
        {
            relativeOrders = new long[] { 7, 24, 60, 60, 1 };
            orderTitles = new string[] {
                TimeUnits.Weeks,
                TimeUnits.Days,
                TimeUnits.Hours,
                TimeUnits.Minutes,
                TimeUnits.Seconds };
            format = "{0:0} {1}";
            zero = TimeUnits.Zero;
            roundValue = true;
        }
    }
}
