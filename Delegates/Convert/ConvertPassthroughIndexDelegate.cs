using Interfaces.Delegates.Convert;

namespace Delegates.Convert
{
    public class ConvertPassthroughIndexDelegate : IConvertDelegate<long, long>
    {
        public long Convert(long data)
        {
            return data;
        }
    }
}