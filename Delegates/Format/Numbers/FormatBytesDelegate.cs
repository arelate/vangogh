using Models.Units;

namespace Delegates.Format.Numbers
{
    // NOTE: This is detected as both leaf and root dependency
    // meaning that it's not used in the dependency chain.
    // This code has been used previously to present bytes
    // and is expected to be used in the future. 
    public class FormatBytesDelegate : FormatNumbersDelegate
    {
        public FormatBytesDelegate()
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