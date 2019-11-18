using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;

using Interfaces.Controllers.File;
using Interfaces.Controllers.Stream;

using Interfaces.Validation;
using Interfaces.Status;
using Interfaces.ValidationResults;

using Models.Units;
using Models.ValidationResults;

namespace Controllers.Validation
{
    public class FileValidationController : IFileValidationController
    {
        IConfirmDelegate<string> confirmValidationExpectedDelegate;
        readonly IFileController fileController;
        IStreamController streamController;
        XmlDocument validationXml;
        IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate;
        IValidationResultController validationResultController;
        IStatusController statusController;

        public FileValidationController(
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IFileController fileController,
            IStreamController streamController,
            IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate,
            IValidationResultController validationResultController,
            IStatusController statusController)
        {
            this.confirmValidationExpectedDelegate = confirmValidationExpectedDelegate;
            this.fileController = fileController;
            this.streamController = streamController;
            this.convertBytesToHashDelegate = convertBytesToHashDelegate;
            this.validationResultController = validationResultController;
            this.statusController = statusController;

            validationXml = new XmlDocument { PreserveWhitespace = false };
        }

        public async Task<IFileValidationResults> ValidateFileAsync(string productFileUri, string validationUri, IStatus status)
        {
            var fileValidation = new FileValidation
            {
                Filename = productFileUri
            };

            if (string.IsNullOrEmpty(productFileUri))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(validationUri))
                throw new ArgumentNullException();

            fileValidation.ValidationExpected = confirmValidationExpectedDelegate.Confirm(productFileUri);

            if (!fileValidation.ValidationExpected)
                return fileValidation;

            fileValidation.ValidationFileExists = VerifyValidationFileExists(validationUri);
            fileValidation.ProductFileExists = VerifyProductFileExists(productFileUri);

            if (!fileValidation.ValidationFileExists ||
                !fileValidation.ProductFileExists)
                return fileValidation;

            try
            {
                using (var xmlStream = streamController.OpenReadable(validationUri))
                    validationXml.Load(xmlStream);

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

                    chunksValidation.Add(await VerifyChunkAsync(fileStream, from, to, expectedMd5, status));

                    await statusController.UpdateProgressAsync(
                        status,
                        length,
                        expectedSize,
                        productFileUri,
                        DataUnits.Bytes);
                }

                fileValidation.Chunks = chunksValidation.ToArray();

                await statusController.UpdateProgressAsync(status, length, expectedSize, productFileUri);
            }

            await statusController.InformAsync(status, $"Validation result: {productFileUri} is valid: " +
                $"{validationResultController.ProductFileIsValid(fileValidation)}");

            return fileValidation;
        }

        public async Task<IChunkValidation> VerifyChunkAsync(System.IO.Stream fileStream, long from, long to, string expectedMd5, IStatus status)
        {
            if (!fileStream.CanSeek)
                throw new Exception("Unable to seek in the file stream");

            var chunkValidation = new ChunkValidation
            {
                From = from,
                To = to,
                ExpectedHash = expectedMd5
            };

            fileStream.Seek(from, SeekOrigin.Begin);

            var length = (int)(to - from + 1);
            byte[] buffer = new byte[length];
            await fileStream.ReadAsync(buffer, 0, length);

            chunkValidation.ActualHash = await convertBytesToHashDelegate.ConvertAsync(buffer, status);

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
