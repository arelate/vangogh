using Interfaces.Hash;
using Interfaces.Conversion;

namespace Controllers.Hash
{
    public class StringMd5Controller : IStringHashController
    {
        private IConversionController<string, byte[]> stringToBytesConversionController;
        private IBytesHashController bytesHashController;

        public StringMd5Controller(
            IConversionController<string, byte[]> stringToBytesConversionController,
            IBytesHashController bytesHashController)
        {
            this.stringToBytesConversionController = stringToBytesConversionController;
            this.bytesHashController = bytesHashController;
        }

        public string GetHash(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var bytes = stringToBytesConversionController.Convert(data);

            if (bytes == null) return string.Empty;
            if (bytes.Length == 0) return string.Empty;

            return bytesHashController.GetHash(bytes);
        }
    }
}
