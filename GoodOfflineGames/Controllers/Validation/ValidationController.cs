using System;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

using Interfaces.Validation;
using Interfaces.Destination;
using Interfaces.File;

namespace Controllers.Validation
{
    public class ValidationController : IValidationController
    {
        private MD5CryptoServiceProvider md5CryptoServiceProvider;
        private IDestinationController validationDestinationController;
        private IFileController fileController;
        private XmlDocument validationXml;
        private System.IO.Stream fileStream;

        private static class ValidationXmlMetadata
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

        public ValidationController(
            IDestinationController validationDestinationController,
            IFileController fileController)
        {
            this.validationDestinationController = validationDestinationController;
            this.fileController = fileController;

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

            //await ValidateChunk(null, 0, 0, null);
        }

        public Task ValidateChunk(System.IO.Stream fileStream, long from, long to, string expectedMd5)
        {
            throw new NotImplementedException();
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
