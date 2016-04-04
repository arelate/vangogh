using System;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IGetValidationUriDelegate
    {
        Uri GetValidationUri(string resolvedUri);
    }

    public interface IGetLocalValidationFilenameForUriDelegate
    {
        string GetLocalValidationFilename(Uri validationUri);
    }

    public interface IGetLocalValidationFilenameForLocalPathDelegate
    {
        string GetLocalValidationFilename(string localPath);
    }

    public interface IDownloadValidationFileDelegate
    {
        Task<bool> DownloadValidationFile(Uri validationUri);
    }

    public interface IValidateSizeDelegate
    {
        bool ValidateSize(string uri, long expectedSize);
    }

    public interface IValidateNameDelegate
    {
        bool ValidateName(string uri, string expectedFilename);
    }

    public interface IBytesToHexStringDelegate
    {
        string BytesToHexString(byte[] data);
    }

    public interface IValidateChunkDelegate
    {
        Task<bool> ValidateChunk(Stream productFileStream, XmlNode chunkElement);
    }

    public interface IValidateProductFileDataDelegate
    {
        Task<Tuple<bool, string>> ValidateProductFileData(ProductFile productFile, XmlDocument validationData);
    }

    public interface IValidateProductFileDelegate
    {
        Task<Tuple<bool, string>> ValidateProductFile(ProductFile productFile);
    }

    public interface IFileValidationController :
        IGetValidationUriDelegate,
        IGetLocalValidationFilenameForUriDelegate,
        IGetLocalValidationFilenameForLocalPathDelegate,
        IDownloadValidationFileDelegate,
        IValidateSizeDelegate,
        IValidateNameDelegate,
        IBytesToHexStringDelegate,
        IValidateChunkDelegate,
        IValidateProductFileDataDelegate,
        IValidateProductFileDelegate
    {
        // ...
    }

}
