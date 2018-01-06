using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Hash;

namespace Controllers.Hash
{
    public class StringMd5Controller : IStringHashController
    {
        private IConvertDelegate<string, byte[]> stringToBytesConversionController;
        private IBytesHashController bytesHashController;

        public StringMd5Controller(
            IConvertDelegate<string, byte[]> stringToBytesConversionController,
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
