using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions
{
    public class ConvertPassthroughIndexDelegate : IConvertDelegate<long, long>
    {
        public long Convert(long data)
        {
            return data;
        }
    }
}