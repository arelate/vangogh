using System.Security.Cryptography;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Hash;

using Interfaces.Status;

namespace Delegates.Hash
{
    public class GetBytesMd5HashAsyncDelegate: IGetHashAsyncDelegate<byte[]>
    {
        private MD5 md5;
        private IConvertDelegate<byte[], string> byteToStringConversionController;

        public GetBytesMd5HashAsyncDelegate(
            IConvertDelegate<byte[], string> byteToStringConversionController)
        {
            md5 = MD5.Create();
            this.byteToStringConversionController = byteToStringConversionController;
        }

        public async Task<string> GetHashAsync(byte[] data, IStatus status)
        {
            return await Task.Run(() => {
                if (data == null)
                    return string.Empty;

                var hashData = md5.ComputeHash(data);
                return byteToStringConversionController.Convert(hashData);    
            });
        }
    }
}
