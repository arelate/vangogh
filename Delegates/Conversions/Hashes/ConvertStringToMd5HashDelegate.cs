using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Hashes
{
    public class ConvertStringToMd5HashDelegate : IConvertAsyncDelegate<string, Task<string>>
    {
        private readonly IConvertDelegate<string, byte[]> convertStringToBytesDelegate;
        private readonly IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate;

        [Dependencies(
            typeof(Bytes.ConvertStringToBytesDelegate),
            typeof(ConvertBytesToMd5HashDelegate))]
        public ConvertStringToMd5HashDelegate(
            IConvertDelegate<string, byte[]> convertStringToBytesDelegate,
            IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate)
        {
            this.convertStringToBytesDelegate = convertStringToBytesDelegate;
            this.convertBytesToHashDelegate = convertBytesToHashDelegate;
        }

        public async Task<string> ConvertAsync(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var bytes = convertStringToBytesDelegate.Convert(data);

            if (bytes == null) return string.Empty;
            if (bytes.Length == 0) return string.Empty;

            return await convertBytesToHashDelegate.ConvertAsync(bytes);
        }
    }
}