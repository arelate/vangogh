using System.Security.Cryptography;

using Interfaces.Hash;
using Interfaces.Conversion;

namespace Controllers.Hash
{
    public class BytesMd5Controller : IBytesHashController
    {
        private MD5 md5;
        private IConversionController<byte[], string> byteToStringConversionController;

        public BytesMd5Controller(IConversionController<byte[], string> byteToStringConversionController)
        {
            md5 = MD5.Create();
            this.byteToStringConversionController = byteToStringConversionController;
        }

        public string GetHash(byte[] data)
        {
            if (data == null)
                return string.Empty;

            var hashData = md5.ComputeHash(data);
            return byteToStringConversionController.Convert(hashData);
        }
    }
}
