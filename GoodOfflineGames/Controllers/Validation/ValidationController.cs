﻿using System;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using Interfaces.Validation;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.File;
using Interfaces.Stream;
using Interfaces.Hash;
using Interfaces.Status;

using Models.ValidationChunk;
using Models.Units;

namespace Controllers.Validation
{
    public class ValidationController : IValidationController
    {
        //private IGetDirectoryDelegate getDirectoryDelegate;
        //private IGetFilenameDelegate getFilenameDelegate;
        private IFileController fileController;
        private IStreamController streamController;
        private XmlDocument validationXml;
        private IBytesToStringHashController bytesToStringHasController;
        private IStatusController statusController;

        public ValidationController(
            //IGetDirectoryDelegate getDirectoryDelegate,
            //IGetFilenameDelegate getFilenameDelegate,
            IFileController fileController,
            IStreamController streamController,
            IBytesToStringHashController bytesToStringHasController,
            IStatusController statusController)
        {
            //this.getDirectoryDelegate = getDirectoryDelegate;
            //this.getFilenameDelegate = getFilenameDelegate;
            this.fileController = fileController;
            this.streamController = streamController;
            this.bytesToStringHasController = bytesToStringHasController;
            this.statusController = statusController;

            validationXml = new XmlDocument()  { PreserveWhitespace = false };
        }

        public async Task ValidateAsync(string uri, string validationUri, IStatus status)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("File location is invalid");

            //var validationFilename = Path.Combine(
            //    getDirectoryDelegate.GetDirectory(),
            //    getFilenameDelegate.GetFilename(uri));

            //if (!fileController.Exists(validationFilename))
            //    return;

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

                    statusController.UpdateProgress(
                        status, 
                        length, 
                        expectedSize, 
                        uri, 
                        DataUnits.Bytes);
                }

                statusController.UpdateProgress(status, length, expectedSize, uri);
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

            var computedMD5 = bytesToStringHasController.GetHash(buffer);

            if (computedMD5 != chunk.ExpectedMD5)
                throw new Exception($"Chunk {chunk.From}-{chunk.To} failed validation");
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
