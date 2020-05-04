using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Attributes;
using Delegates.Conversions.Hashes;
using Delegates.Conversions.Streams;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Confirmations.Validation
{
    // TODO: This needs to be refactored into smaller delegates
    public class ConfirmFileValidationExpectationsAsyncDelegate : IConfirmExpectationAsyncDelegate<string, string>
    {
        private readonly IConfirmDelegate<string> confirmValidationExpectedDelegate;
        private readonly IConfirmDelegate<string> confirmFileExistsDelegate;
        private readonly IConfirmExpectationDelegate<string, string> confirmFilenameExpectationDelegate;
        private readonly IConfirmExpectationDelegate<string, long> confirmFileSizeExpectationDelegate;

        private readonly IConfirmExpectationAsyncDelegate<(Stream FileStream, long From, long To), string>
            confirmChunkHashExpectationAsyncDelegate;
        
        private readonly IConvertDelegate<string, Stream> convertUriToReadableStreamDelegate;
        private XmlDocument validationXml;
        private IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate;

        [Dependencies(
            typeof(ConfirmFileValidationSupportedDelegate),
            typeof(ConfirmFileExistsDelegate),
            typeof(ConfirmFilenameExpectationDelegate),
            typeof(ConfirmSizeExpectationDelegate),
            typeof(ConfirmChunkHashExpectationAsyncDelegate),
            typeof(ConvertUriToReadableStreamDelegate),
            typeof(ConvertBytesToMd5HashDelegate))]
        public ConfirmFileValidationExpectationsAsyncDelegate(
            IConfirmDelegate<string> confirmValidationExpectedDelegate,
            IConfirmDelegate<string> confirmFileExistsDelegate,
            IConfirmExpectationDelegate<string, string> confirmFilenameExpectationDelegate,
            IConfirmExpectationDelegate<string, long> confirmFileSizeExpectationDelegate,
            IConfirmExpectationAsyncDelegate<(Stream FileStream, long From, long To), string>
                confirmChunkHashExpectationAsyncDelegate,
            IConvertDelegate<string, Stream> convertUriToReadableStreamDelegate,
            IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashDelegate)
        {
            this.confirmValidationExpectedDelegate = confirmValidationExpectedDelegate;
            this.confirmFileExistsDelegate = confirmFileExistsDelegate;
            this.confirmFilenameExpectationDelegate = confirmFilenameExpectationDelegate;
            this.confirmFileSizeExpectationDelegate = confirmFileSizeExpectationDelegate;
            this.confirmChunkHashExpectationAsyncDelegate = confirmChunkHashExpectationAsyncDelegate;
            
            this.convertUriToReadableStreamDelegate = convertUriToReadableStreamDelegate;
            this.convertBytesToHashDelegate = convertBytesToHashDelegate;

            validationXml = new XmlDocument {PreserveWhitespace = false};
        }

        public async Task<bool> ConfirmAsync(string productFileUri, string validationUri)
        {
            if (string.IsNullOrEmpty(productFileUri) ||
                string.IsNullOrEmpty(validationUri))
                throw new ArgumentNullException();

            if (!confirmValidationExpectedDelegate.Confirm(productFileUri)) return true;

            if (!confirmFileExistsDelegate.Confirm(validationUri)) return false;
            if (!confirmFileExistsDelegate.Confirm(productFileUri)) return false;

            try
            {
                using (var xmlStream = convertUriToReadableStreamDelegate.Convert(validationUri))
                {
                    validationXml.Load(xmlStream);
                }
            }
            catch
            {
                return false;
            }

            var fileElement = validationXml.GetElementsByTagName("file");
            if (fileElement.Count < 1 ||
                fileElement[0] == null ||
                fileElement[0].Attributes == null)
            {
                return false;
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
                return false;
            }

            if (!confirmFilenameExpectationDelegate.Confirm(productFileUri, expectedName)) return false;
            if (!confirmFileSizeExpectationDelegate.Confirm(productFileUri, expectedSize)) return false;

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

                    if (!await confirmChunkHashExpectationAsyncDelegate.ConfirmAsync(
                        (fileStream, from, to), 
                        expectedMd5)) return false;

                    // await statusController.UpdateProgressAsync(
                    //     status,
                    //     length,
                    //     expectedSize,
                    //     productFileUri,
                    //     DataUnits.Bytes);
                }

                // await statusController.UpdateProgressAsync(status, length, expectedSize, productFileUri);
            }

            // await statusController.InformAsync(status, $"Validation result: {productFileUri} is valid: " +
            //     $"{validationResultController.ProductFileIsValid(fileValidation)}");

            return true;
        }
    }
}