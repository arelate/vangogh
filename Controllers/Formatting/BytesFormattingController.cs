using Models.Units;

namespace Controllers.Formatting
{
    public class BytesFormattingController : FormattingController
    {
        public BytesFormattingController()
        {
            relativeOrders = new long[] { 1024, 1024, 1024, 1024, 1 };
            orderTitles = new string[] {
                DataUnits.Terabytes,
                DataUnits.Gigabytes,
                DataUnits.Megabytes,
                DataUnits.Kilobytes,
                DataUnits.Bytes
            };
            format = "{0:0.0} {1}";
            zero = DataUnits.Zero;
        }
    }
}
