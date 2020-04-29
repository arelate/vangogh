using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Activities;
using Interfaces.Validation;
using Interfaces.ValidationResults;
using Attributes;
using Models.Units;
using Models.ProductTypes;

namespace Controllers.Validation
{
    public class FileValidationController : IFileValidationController
    {
        private IConfirmDelegate<string> confirmValidationExpectedDelegate;
        private readonly IConvertDelegate<string, Stream> convertUriToReadableStreamDelegate;
        private XmlDocument validationXml;
        private IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate;
        private IValidationResultController validationResultController;

        [Dependencies(
            "Delegates.Confirm.ConfirmValidationExpectedDelegate,Delegates",
            "Delegates.Convert.Streams.ConvertUriToReadableStreamDelegate,Delegates",
            "Delegates.Convert.Hashes.ConvertBytesToMd5HashDelegate,Delegates",
            "Controllers.ValidationResult.ValidationResultController,Controllers")]
        public FileValidationController(
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IConvertDelegate<string, Stream> convertUriToReadableStreamDelegate,
            IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate,
            IValidationResultController validationResultController)
        {
            this.confirmValidationExpectedDelegate = confirmValidationExpectedDelegate;
            this.convertUriToReadableStreamDelegate = convertUriToReadableStreamDelegate;
            this.convertBytesToHashDelegate = convertBytesToHashDelegate;
            this.validationResultController = validationResultController;

            validationXml = new XmlDocument {PreserveWhitespace = false};
        }

        public async Task<IFileValidationResults> ValidateFileAsync(string productFileUri, string validationUri)
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
                using (var xmlStream = convertUriToReadableStreamDelegate.Convert(validationUri))
                {
                    validationXml.Load(xmlStream);
                }

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

            using (var fileStream = convertUriToReadableStreamDelegate.Convert(productFileUri))
            {
                long length = 0;

                foreach (XmlNode chunkElement in fileElement[0].ChildNodes)
                {
                    if (chunkElement.Name != "chunk")
                        continue;

                    long from, to = 0;
                    var expectedMd5 = string.Empty;

                    from = long.Parse(chunkElement.Attributes["from"]?.Value);
                    to = long.Parse(chunkElement.Attributes["to"]?.Value);
                    length += to - @from;
                    expectedMd5 = chunkElement.FirstChild.Value;

                    chunksValidation.Add(await VerifyChunkAsync(fileStream, from, to, expectedMd5));

                    // await statusController.UpdateProgressAsync(
                    //     status,
                    //     length,
                    //     expectedSize,
                    //     productFileUri,
                    //     DataUnits.Bytes);
                }

                fileValidation.Chunks = chunksValidation.ToArray();

                // await statusController.UpdateProgressAsync(status, length, expectedSize, productFileUri);
            }

            // await statusController.InformAsync(status, $"Validation result: {productFileUri} is valid: " +
            //     $"{validationResultController.ProductFileIsValid(fileValidation)}");

            return fileValidation;
        }
    }
}