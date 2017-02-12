using System.Security.Cryptography;

using Interfaces.Hash;
using Interfaces.Conversion;

namespace Controllers.Hash
{
    public class BytesToStringMd5HashController : IBytesToStringHashController
    {
        private MD5 md5;
        private IConversionController<byte[], string> byteToStringConversionController;

        public BytesToStringMd5HashController(IConversionController<byte[], string> byteToStringConversionController)
        {
            md5 = MD5.Create();
            this.byteToStringConversionController = byteToStringConversionController;
        }

        public string ComputeHash(byte[] data)
        {
            if (data == null)
                return string.Empty;

            var hashData = md5.ComputeHash(data);
            return byteToStringConversionController.Convert(hashData);
        }
    }
}
