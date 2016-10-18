using System;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

using Interfaces.Validation;
using Interfaces.Destination;
using Interfaces.File;
using Interfaces.Conversion;
using Interfaces.Stream;

namespace Controllers.Validation
{
    static class ValidationXmlMetadata
    {
        public const string File = "file";
        // file element attributes
        public const string TotalSize = "total_size";
        public const string Timestamp = "timestamp";
        public const string Chunks = "chunks";
        public const string Available = "available";
        public const string NotAvailableMessage = "notavailablemsg";
        public const string Name = "name";
        // chunk elements attributes
        public const string Id = "id";
        public const string From = "from";
        public const string To = "to";
    }

    public class ValidationController : IValidationController
    {
        private MD5CryptoServiceProvider md5CryptoServiceProvider;
        private IDestinationController validationDestinationController;
        private IFileController fileController;
        private IStreamController streamController;
        private XmlDocument validationXml;
        //private System.IO.Stream fileStream;
        private IConversionController<byte[], string> byteToStringConversionController;

        public ValidationController(
            IDestinationController validationDestinationController,
            IFileController fileController,
            IStreamController streamController,
            IConversionController<byte[], string> byteToStringConversionController)
        {
            this.validationDestinationController = validationDestinationController;
            this.fileController = fileController;
            this.streamController = streamController;
            this.byteToStringConversionController = byteToStringConversionController;

            validationXml = new XmlDocument();

            md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            md5CryptoServiceProvider.Initialize();
        }

        public async Task Validate(string uri)
        {
            var validationFilename = Path.Combine(
                validationDestinationController.GetDirectory(uri),
                validationDestinationController.GetFilename(uri));

            validationXml.Load(validationFilename);

            var fileElement = validationXml.GetElementsByTagName(ValidationXmlMetadata.File);
            if (fileElement == null ||
                fileElement.Count < 1 ||
                fileElement[0] == null ||
                fileElement[0].Attributes == null)
                throw new Exception("Validation XML is invalid");

            long expectedSize;
            string expectedName;
            int chunks;
            bool available;

            try
            {
                expectedSize = long.Parse(fileElement[0].Attributes[ValidationXmlMetadata.TotalSize]?.Value);
                expectedName = fileElement[0].Attributes[ValidationXmlMetadata.Name]?.Value;
                chunks = int.Parse(fileElement[0].Attributes[ValidationXmlMetadata.Chunks]?.Value);
                available = fileElement[0].Attributes[ValidationXmlMetadata.Available]?.Value == "1";

                if (!available)
                {
                    var notAvailableMessage = fileElement[0].Attributes[ValidationXmlMetadata.NotAvailableMessage]?.Value;
                    throw new Exception(notAvailableMessage);
                }
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Validation XML 'file' element attribute is null");
            }
            catch (FormatException)
            {
                throw new FormatException("Validation XML 'file' element attribute contain data in unsupported format");
            }
            catch (OverflowException)
            {
                throw new OverflowException("Validation XML 'file' element attribute contain data that cannot be processed");
            }

            ValidateFilename(uri, expectedName);

            ValidateSize(uri, expectedSize);

            using (var fileStream = streamController.OpenReadable(uri))
            {
                foreach (XmlNode chunkElement in fileElement[0].ChildNodes)
                {
                    long from, to = 0;
                    string expectedMd5 = string.Empty;

                    from = long.Parse(chunkElement.Attributes[ValidationXmlMetadata.From]?.Value);
                    to = long.Parse(chunkElement.Attributes[ValidationXmlMetadata.To]?.Value);
                    expectedMd5 = chunkElement.FirstChild.Value;

                    await ValidateChunk(fileStream, from, to, expectedMd5);
                }
            }
        }

        public async Task ValidateChunk(System.IO.Stream fileStream, long from, long to, string expectedMd5)
        {
            if (!fileStream.CanSeek)
                throw new Exception("Unable to seek in the file stream");

            fileStream.Seek(from, SeekOrigin.Begin);

            var length = (int)(to - from + 1);
            byte[] buffer = new byte[length];
            await fileStream.ReadAsync(buffer, 0, length);

            md5CryptoServiceProvider.Initialize();
            md5CryptoServiceProvider.TransformFinalBlock(buffer, 0, length);

            byte[] hash = md5CryptoServiceProvider.Hash;

            var computedMD5 = byteToStringConversionController.Convert(hash);

            if (computedMD5 != expectedMd5)
                throw new Exception(
                    string.Format(
                        "Chunk {0}-{1} failed validation", 
                        from,
                        to));
        }

        public void ValidateFilename(string uri, string expectedFilename)
        {
            if (Path.GetFileName(uri) != expectedFilename)
                throw new Exception("Filename doesn't match expected value");
        }

        public void ValidateSize(string uri, long expectedSize)
        {
            if (fileController.GetSize(uri) != expectedSize)
                throw new Exception("File size doesn't match expected size");
        }
    }
}
