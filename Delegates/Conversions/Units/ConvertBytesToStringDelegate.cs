using Models.Units;

namespace Delegates.Conversions.Units
{
    // NOTE: Don't remove this class that is not used.
    // This code has been used previously to present bytes
    // and is expected to be used in the future. 
    public class ConvertBytesToStringDelegate : ConvertNumberToStringDelegate
    {
        public ConvertBytesToStringDelegate()
        {
            relativeOrders = new long[] {1024, 1024, 1024, 1024, 1};
            orderTitles = new string[]
            {
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