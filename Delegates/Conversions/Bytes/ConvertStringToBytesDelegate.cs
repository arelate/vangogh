using System.Text;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Bytes
{
    public class ConvertStringToBytesDelegate : IConvertDelegate<string, byte[]>
    {
        public byte[] Convert(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new byte[0];

            return Encoding.UTF8.GetBytes(data);
        }
    }
}