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
using Interfaces.TaskStatus;

using Models.ValidationChunk;

namespace Controllers.Validation
{
    public class ValidationController : IValidationController
    {
        private MD5CryptoServiceProvider md5CryptoServiceProvider;
        private IDestinationController validationDestinationController;
        private IFileController fileController;
        private IStreamController streamController;
        private XmlDocument validationXml;
        private IConversionController<byte[], string> byteToStringConversionController;
        private ITaskStatusController taskStatusController;

        public ValidationController(
            IDestinationController validationDestinationController,
            IFileController fileController,
            IStreamController streamController,
            IConversionController<byte[], string> byteToStringConversionController,
            ITaskStatusController taskStatusController)
        {
            this.validationDestinationController = validationDestinationController;
            this.fileController = fileController;
            this.streamController = streamController;
            this.byteToStringConversionController = byteToStringConversionController;
            this.taskStatusController = taskStatusController;

            validationXml = new XmlDocument()  { PreserveWhitespace = false };

            md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            md5CryptoServiceProvider.Initialize();
        }

        public async Task ValidateAsync(string uri, ITaskStatus taskStatus)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("File location is invalid");

            var validationFilename = Path.Combine(
                validationDestinationController.GetDirectory(uri),
                validationDestinationController.GetFilename(uri));

            if (!fileController.Exists(validationFilename))
                return;

            validationXml.Load(validationFilename);

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

            ValidateFilename(uri, expectedName);

            ValidateSize(uri, expectedSize);

            using (var fileStream = streamController.OpenReadable(uri))
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

                    var chunk = new ValidationChunk()
                    {
                        From = from,
                        To = to,
                        ExpectedMD5 = expectedMd5
                    };

                    await ValidateChunkAsync(fileStream, chunk);

                    taskStatusController.UpdateProgress(taskStatus, length, expectedSize, uri, "byte(s)");
                }

                taskStatusController.UpdateProgress(taskStatus, length, expectedSize, uri);
            }
        }

        public async Task ValidateChunkAsync(System.IO.Stream fileStream, IValidationChunk chunk)
        {
            if (!fileStream.CanSeek)
                throw new Exception("Unable to seek in the file stream");

            fileStream.Seek(chunk.From, SeekOrigin.Begin);

            var length = (int)(chunk.To - chunk.From + 1);
            byte[] buffer = new byte[length];
            await fileStream.ReadAsync(buffer, 0, length);

            md5CryptoServiceProvider.Initialize();
            md5CryptoServiceProvider.TransformFinalBlock(buffer, 0, length);

            byte[] hash = md5CryptoServiceProvider.Hash;

            var computedMD5 = byteToStringConversionController.Convert(hash);

            if (computedMD5 != chunk.ExpectedMD5)
                throw new Exception(
                    string.Format(
                        "Chunk {0}-{1} failed validation",
                        chunk.From,
                        chunk.To));
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
