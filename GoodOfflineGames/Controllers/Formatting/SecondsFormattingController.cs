namespace Controllers.Formatting
{
    public class SecondsFormattingController : FormattingController
    {
        public SecondsFormattingController()
        {
            relativeOrders = new long[] { 7, 24, 60, 60, 1 };
            orderTitles = new string[] { "week(s)", "day(s)", "hour(s)", "minute(s)", "second(s)" };
            format = "{0:0} {1}";
            zero = "zero seconds";
            roundValue = true;
        }
    }
}
