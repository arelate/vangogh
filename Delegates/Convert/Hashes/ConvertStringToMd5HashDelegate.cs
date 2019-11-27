using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

namespace Delegates.Convert.Hashes
{
    public class ConvertStringToMd5HashDelegate : IConvertAsyncDelegate<string, Task<string>>
    {
        readonly IConvertDelegate<string, byte[]> convertStringToBytesDelegate;
        readonly IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate;

        [Dependencies(
            "Delegates.Convert.Bytes.ConvertStringToBytesDelegate,Delegates",
            "Delegates.Convert.Hashes.ConvertBytesToMd5HashDelegate,Delegates")]
        public ConvertStringToMd5HashDelegate(
            IConvertDelegate<string, byte[]> convertStringToBytesDelegate,
            IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate)
        {
            this.convertStringToBytesDelegate = convertStringToBytesDelegate;
            this.convertBytesToHashDelegate = convertBytesToHashDelegate;
        }

        public async Task<string> ConvertAsync(string data, IStatus status)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var bytes = convertStringToBytesDelegate.Convert(data);

            if (bytes == null) return string.Empty;
            if (bytes.Length == 0) return string.Empty;

            return await convertBytesToHashDelegate.ConvertAsync(bytes, status);
        }
    }
}
