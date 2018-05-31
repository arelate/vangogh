using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Hash;

using Interfaces.Status;

namespace Delegates.Hash
{
    public class GetStringMd5HashAsyncDelegate : IGetHashAsyncDelegate<string>
    {
        readonly IConvertDelegate<string, byte[]> stringToBytesConversionController;
        readonly IGetHashAsyncDelegate<byte[]> getBytesHashAsyncDelegate;

        public GetStringMd5HashAsyncDelegate(
            IConvertDelegate<string, byte[]> stringToBytesConversionController,
            IGetHashAsyncDelegate<byte[]> getBytesHashAsyncDelegate)
        {
            this.stringToBytesConversionController = stringToBytesConversionController;
            this.getBytesHashAsyncDelegate = getBytesHashAsyncDelegate;
        }

        public async Task<string> GetHashAsync(string data, IStatus status)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var bytes = stringToBytesConversionController.Convert(data);

            if (bytes == null) return string.Empty;
            if (bytes.Length == 0) return string.Empty;

            return await getBytesHashAsyncDelegate.GetHashAsync(bytes, status);
        }
    }
}
