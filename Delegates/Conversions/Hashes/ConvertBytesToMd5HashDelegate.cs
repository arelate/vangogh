using System.Security.Cryptography;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Hashes
{
    public class ConvertBytesToMd5HashDelegate : IConvertAsyncDelegate<byte[], Task<string>>
    {
        private readonly MD5 md5;
        private readonly IConvertDelegate<byte[], string> byteToStringConversionController;

        [Dependencies(
            typeof(Bytes.ConvertBytesToStringDelegate))]
        public ConvertBytesToMd5HashDelegate(
            IConvertDelegate<byte[], string> byteToStringConversionController)
        {
            md5 = MD5.Create();
            this.byteToStringConversionController = byteToStringConversionController;
        }

        public async Task<string> ConvertAsync(byte[] data)
        {
            return await Task.Run(() =>
            {
                if (data == null)
                    return string.Empty;

                var hashData = md5.ComputeHash(data);
                return byteToStringConversionController.Convert(hashData);
            });
        }
    }
}