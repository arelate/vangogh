using System.Text;

using Interfaces.Conversion;

namespace Controllers.Conversion
{
    public class BytesToStringConvertionController : IConversionController<byte[], string>
    {
        public string Convert(byte[] data)
        {
            var stringBuilder = new StringBuilder();

            for (var ii = 0; ii < data.Length; ii++)
                stringBuilder.Append(data[ii].ToString("x2"));

            return stringBuilder.ToString();
        }
    }
}
