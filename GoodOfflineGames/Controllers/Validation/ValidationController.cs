using System;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using Interfaces.Validation;
using Interfaces.File;
using Interfaces.Stream;
using Interfaces.Hash;
using Interfaces.Status;
using Interfaces.Expectation;

using Models.Units;

namespace Controllers.Validation
{
    public class ValidationController : IValidationController
    {
        private IExpectedDelegate<string> validationExpectedDelegate;
        private IFileController fileController;
        private IStreamController streamController;
        private XmlDocument validationXml;
        private IBytesToStringHashController bytesToStringHasController;
        private IStatusController statusController;

        public ValidationController(
            IExpectedDelegate<string> validationExpectedDelegate,
            IFileController fileController,
            IStreamController streamController,
            IBytesToStringHashController bytesToStringHasController,
            IStatusController statusController)
        {
            this.validationExpectedDelegate = validationExpectedDelegate;
            this.fileController = fileController;
            this.streamController = streamController;
            this.bytesToStringHasController = bytesToStringHasController;
            this.statusController = statusController;

            validationXml = new XmlDocument()  { PreserveWhitespace = false };
        }

        public async Task ValidateAsync(string localFileUri, string validationUri, IStatus status)
        {
            if (string.IsNullOrEmpty(localFileUri))
                throw new ArgumentNullException("File location is invalid");

            if (!validationExpectedDelegate.Expected(localFileUri))
                return;

            validationXml.Load(validationUri);

            var fileElement = validationXml.GetElementsByTagName("file");
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
                expectedSize = long.Parse(fileElement[0].Attributes["total_size"]?.Value);
                expectedName = fileElement[0].Attributes["name"]?.Value;
                chunks = int.Parse(fileElement[0].Attributes["chunks"]?.Value);
                available = fileElement[0].Attributes["available"]?.Value == "1";

                if (!available)
                {
                    var notAvailableMessage = fileElement[0].Attributes["notavailablemsg"]?.Value;
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

            VerifyFilename(localFileUri, expectedName);

            VerifySize(localFileUri, expectedSize);

            using (var fileStream = streamController.OpenReadable(localFileUri))
            {
                long length = 0;

                foreach (XmlNode chunkElement in fileElement[0].ChildNodes)
                {
                    if (chunkElement.Name != "chunk")
                        continue;

                    long from, to = 0;
                    string expectedMd5 = string.Empty;

                    from = long.Parse(chunkElement.Attributes["from"]?.Value);
                    to = long.Parse(chunkElement.Attributes["to"]?.Value);
                    length += (to - from);
                    expectedMd5 = chunkElement.FirstChild.Value;

                    await VerifyChunkAsync(fileStream, from, to, expectedMd5);

                    statusController.UpdateProgress(
                        status, 
                        length, 
                        expectedSize,
                        localFileUri, 
                        DataUnits.Bytes);
                }

                statusController.UpdateProgress(status, length, expectedSize, localFileUri);
            }
        }

        public async Task VerifyChunkAsync(System.IO.Stream fileStream, long from, long to, string expectedMd5)
        {
            if (!fileStream.CanSeek)
                throw new Exception("Unable to seek in the file stream");

            fileStream.Seek(from, SeekOrigin.Begin);

            var length = (int)(to - from + 1);
            byte[] buffer = new byte[length];
            await fileStream.ReadAsync(buffer, 0, length);

            var computedMd5 = bytesToStringHasController.GetHash(buffer);

            if (computedMd5 != expectedMd5)
                throw new Exception($"Chunk {from}-{to} failed validation");
        }

        public void VerifyFilename(string uri, string expectedFilename)
        {
            if (Path.GetFileName(uri) != expectedFilename)
                throw new Exception("Filename doesn't match expected value");
        }

        public void VerifySize(string uri, long expectedSize)
        {
            if (fileController.GetSize(uri) != expectedSize)
                throw new Exception("File size doesn't match expected size");
        }
    }
}
