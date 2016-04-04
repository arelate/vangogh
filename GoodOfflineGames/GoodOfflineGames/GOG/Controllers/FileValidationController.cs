using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    public class FileValidationController: IFileValidationController
    {
        private const string validationFilesContainer = "_md5";

        // file element attributes
        private const string totalSizeAttribute = "total_size";
        private const string timestampAttribute = "timestamp";
        private const string chunksAttribute = "chunks";
        private const string availableAttribute = "available";
        private const string notAvailableMessageAttribute = "notavailablemsg";
        private const string nameAttribute = "name";
        // chunk elements attributes
        private const string idAttribute = "id";
        private const string fromAttribute = "from";
        private const string toAttribute = "to";

        // only validate binary files
        private readonly string[] extensionsWhitelist = new string[3] { ".exe", ".bin", ".dmg" };

        private IIOController ioController;
        private IRequestFileDelegate requestFileDelegate;
        private IPostUpdateDelegate postUpdateDelegate;

        private MD5CryptoServiceProvider md5CryptoServiceProvider;

        public FileValidationController(
            IIOController ioController,
            IRequestFileDelegate requestFileDelegate,
            IPostUpdateDelegate postUpdateDelegate)
        {
            this.ioController = ioController;
            this.requestFileDelegate = requestFileDelegate;
            this.postUpdateDelegate = postUpdateDelegate;

            md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            md5CryptoServiceProvider.Initialize();
        }

        public Uri GetValidationUri(string resolvedUri)
        {
            const string replaceEntry = "?";
            const string replaceWithEntry = ".xml";

            if (string.IsNullOrEmpty(resolvedUri)) return null;

            var validationUri = (resolvedUri.Contains(replaceEntry)) ?
                new Uri(resolvedUri.Replace(replaceEntry, replaceWithEntry + replaceEntry)) :
                new Uri(resolvedUri + replaceWithEntry);

            return validationUri;
        }

        public string GetLocalValidationFilename(Uri validationUri)
        {
            return Path.Combine(validationFilesContainer,
                validationUri.Segments.Last());
        }

        public async Task<bool> DownloadValidationFile(Uri validationUri)
        {
            string validationFilename = GetLocalValidationFilename(validationUri);
            if (ioController.FileExists(validationFilename))
            {
                return true;
            }
            else
            {
                var result = await requestFileDelegate.RequestFile(
                    validationUri.ToString(),
                    validationFilesContainer,
                    ioController);

                return result.Item1;
            }
        }

        public bool ValidateSize(string uri, long expectedSize)
        {
            return ioController.GetSize(uri) == expectedSize;
        }

        public bool ValidateName(string uri, string expectedFilename)
        {
            return uri == expectedFilename;
        }

        public string BytesToHexString(byte[] data)
        {
            var stringBuilder = new StringBuilder();

            for (var ii = 0; ii < data.Length; ii++)
                stringBuilder.Append(data[ii].ToString("x2"));

            return stringBuilder.ToString();
        }

        public async Task<bool> ValidateChunk(Stream productFileStream, XmlNode chunkElement)
        {
            if (chunkElement == null ||
                chunkElement.FirstChild == null ||
                string.IsNullOrEmpty(chunkElement.FirstChild.Value)) return false;
            if (!productFileStream.CanSeek) return false;

            postUpdateDelegate.PostUpdate();

            long from = 0, to = 0;
            string expectedMD5 = string.Empty;

            try
            {
                from = long.Parse(chunkElement.Attributes[fromAttribute]?.Value);
                to = long.Parse(chunkElement.Attributes[toAttribute]?.Value);
                expectedMD5 = chunkElement.FirstChild.Value;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }

            var length = (int) (to - from + 1);
            byte[] buffer = new byte[length];
            await productFileStream.ReadAsync(buffer, 0, length);

            md5CryptoServiceProvider.Initialize();
            md5CryptoServiceProvider.TransformFinalBlock(buffer, 0, length);

            byte[] hash = md5CryptoServiceProvider.Hash;

            var computedMD5 = BytesToHexString(hash);

            return computedMD5 == expectedMD5;
        }

        public async Task<Tuple<bool, string>> ValidateProductFileData(ProductFile productFile, XmlDocument validationData)
        {
            // validation sequence:
            // - available
            // - filename
            // - size
            // - md5 of each chunk

            if (validationData == null)
                return new Tuple<bool, string>(false, "Validation data is not available.");

            var fileElement = validationData.GetElementsByTagName("file");
            if (fileElement == null ||
                fileElement.Count < 1 ||
                fileElement[0] == null ||
                fileElement[0].Attributes == null)
                return new Tuple<bool, string>(false, "Validation data doesn't contain 'file' element or it's attributes.");

            long expectedSize;
            string expectedName;
            int chunks;
            bool available;

            try
            {
                expectedSize = long.Parse(fileElement[0].Attributes[totalSizeAttribute]?.Value);
                expectedName = fileElement[0].Attributes[nameAttribute]?.Value;
                chunks = int.Parse(fileElement[0].Attributes[chunksAttribute]?.Value);
                available = fileElement[0].Attributes[availableAttribute]?.Value == "1";

                if (!available)
                {
                    var notAvailableMessage = fileElement[0].Attributes[notAvailableMessageAttribute]?.Value;
                    return new Tuple<bool, string>(false, notAvailableMessage);
                }
            }
            catch (ArgumentNullException)
            {
                return new Tuple<bool, string>(false, "Validation data 'file' element attribute is null."); 
            }
            catch (FormatException)
            {
                return new Tuple<bool, string>(false, "Validation data 'file' element attribute contain data in unsupported format.");
            }
            catch (OverflowException)
            {
                return new Tuple<bool, string>(false, "Validation data 'file' element attribute contain data that cannot be processed.");
            }

            var productFileRelativeUri = Path.Combine(productFile.Folder, productFile.File);

            if (!ValidateName(productFile.File, expectedName))
                return new Tuple<bool, string>(false, "Product name doesn't match validation value.");
            if (!ValidateSize(productFileRelativeUri, expectedSize))
                return new Tuple<bool, string>(false, "Product size doesn't match validation value.");

            if (!fileElement[0].HasChildNodes ||
                 fileElement[0].ChildNodes.Count < chunks)
            {
                return new Tuple<bool, string>(false, "Validation data doesn't contain expected number of 'chunk' elements.");
            }

            var result = true;
            using (var productFileStream = ioController.OpenReadable(productFileRelativeUri))
            {
                foreach (XmlNode chunkElement in fileElement[0].ChildNodes)
                {
                    result &= await ValidateChunk(productFileStream, chunkElement);
                }
            }

            if (result)
                return new Tuple<bool, string>(true, string.Empty);

            return new Tuple<bool,string>(false, "Unknown validation error.");
        }

        public async Task<Tuple<bool, string>> ValidateProductFile(ProductFile productFile)
        {
            if (productFile == null)
                return new Tuple<bool, string>(false, "Product data is null.");
            if (!ioController.DirectoryExists(productFile.Folder))
                return new Tuple<bool, string>(false, "Product directory doesn't exist.");

            var productFileExtension = Path.GetExtension(productFile.File);
            if (!extensionsWhitelist.Contains(productFileExtension))
                return new Tuple<bool, string>(false, "File validation is only supported for binary product installers.");

            if (!ioController.FileExists(
                Path.Combine(
                    productFile.Folder,
                    productFile.File)))
                return new Tuple<bool, string>(false, "Product file doesn't exist.");

            var validationUri = GetValidationUri(productFile.ResolvedUrl);

            if (await DownloadValidationFile(validationUri))
            {
                var localValidationFilename = GetLocalValidationFilename(validationUri);

                var validationData = new XmlDocument();
                validationData.Load(localValidationFilename);

                return await ValidateProductFileData(productFile, validationData);
            }
            else return new Tuple<bool, string>(
                false, 
                "There is no validation file available and it couldn't be downloaded.");
        }
    }
}
