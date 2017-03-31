using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Interfaces.Validation;
using Interfaces.File;
using Interfaces.Stream;
using Interfaces.Hash;
using Interfaces.Status;
using Interfaces.Expectation;
using Interfaces.ValidationResult;

using Models.Units;
using Models.ValidationResult;

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

        public async Task<IFileValidation> ValidateAsync(string productFileUri, string validationUri, IStatus status)
        {
            var fileValidation = new FileValidation()
            {
                Filename = productFileUri
            };

            if (string.IsNullOrEmpty(productFileUri))
                throw new ArgumentNullException("File location is invalid");

            if (string.IsNullOrEmpty(validationUri))
                throw new ArgumentNullException("Validation location is invalid");

            fileValidation.ValidationExpected = validationExpectedDelegate.Expected(productFileUri);

            if (!fileValidation.ValidationExpected)
                return fileValidation;

            fileValidation.ValidationFileExists = VerifyValidationFileExists(validationUri);
            fileValidation.ProductFileExists = VerifyProductFileExists(productFileUri);

            try
            {
                validationXml.Load(validationUri);
                fileValidation.ValidationFileIsValid = true;
            }
            catch
            {
                fileValidation.ValidationFileIsValid = false;
                return fileValidation;
            }

            var fileElement = validationXml.GetElementsByTagName("file");
            if (fileElement == null ||
                fileElement.Count < 1 ||
                fileElement[0] == null ||
                fileElement[0].Attributes == null)
            {
                fileValidation.ValidationFileIsValid = false;
                return fileValidation;
            }

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
            catch
            {
                fileValidation.ValidationFileIsValid = false;
                return fileValidation;
            }

            fileValidation.FilenameVerified = VerifyFilename(productFileUri, expectedName);
            fileValidation.SizeVerified = VerifySize(productFileUri, expectedSize);

            var chunksValidation = new List<IChunkValidation>();

            using (var fileStream = streamController.OpenReadable(productFileUri))
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

                    chunksValidation.Add(await VerifyChunkAsync(fileStream, from, to, expectedMd5));

                    statusController.UpdateProgress(
                        status, 
                        length, 
                        expectedSize,
                        productFileUri, 
                        DataUnits.Bytes);
                }

                fileValidation.Chunks = chunksValidation.ToArray();

                statusController.UpdateProgress(status, length, expectedSize, productFileUri);
            }

            return fileValidation;
        }

        public async Task<IChunkValidation> VerifyChunkAsync(System.IO.Stream fileStream, long from, long to, string expectedMd5)
        {
            if (!fileStream.CanSeek)
                throw new Exception("Unable to seek in the file stream");

            var chunkValidation = new ChunkValidation()
            {
                From = from,
                To = to,
                ExpectedHash = expectedMd5
            };

            fileStream.Seek(from, SeekOrigin.Begin);

            var length = (int)(to - from + 1);
            byte[] buffer = new byte[length];
            await fileStream.ReadAsync(buffer, 0, length);

            chunkValidation.ActualHash = bytesToStringHasController.GetHash(buffer);

            return chunkValidation;
        }

        public bool VerifyExpectedValidation(string uri)
        {
            throw new NotImplementedException();
        }

        public bool VerifyFilename(string uri, string expectedFilename)
        {
            return Path.GetFileName(uri) == expectedFilename;
        }

        public bool VerifyProductFileExists(string productFileUri)
        {
            return fileController.Exists(productFileUri);
        }

        public bool VerifySize(string uri, long expectedSize)
        {
            return fileController.GetSize(uri) == expectedSize;
        }

        public bool VerifyValidationFileExists(string validationFileUri)
        {
            return fileController.Exists(validationFileUri);
        }
    }
}
