using System.Threading.Tasks;

using Interfaces.ValidationResult;
using Interfaces.Status;

namespace Interfaces.Validation
{
    public interface IVerifiyExpectedValidationDelegate
    {
        bool VerifyExpectedValidation(string uri);
    }

    public interface IVerifyValidationFileExistsDelegate
    {
        bool VerifyValidationFileExists(string uri);
    }

    public interface IVerifyProductFileExistsDelegate
    {
        bool VerifyProductFileExists(string uri);
    }

    public interface IVerifyFilenameDelegate
    {
        bool VerifyFilename(string uri, string expectedFilename);
    }

    public interface IVerifySizeDelegate
    {
        bool VerifySize(string uri, long expectedSize);
    }

    public interface IVerifyChunkAsyncDelegate
    {
        Task<IChunkValidation> VerifyChunkAsync(System.IO.Stream fileStream, long from, long to, string expectedMd5, IStatus status);
    }

    public interface IValidateFileAsyncDelegate<T>
    {
        Task<T> ValidateFileAsync(string uri, string validationSource, IStatus status);
    }

    public interface IFileValidationController:
        IVerifiyExpectedValidationDelegate,
        IVerifyValidationFileExistsDelegate,
        IVerifyProductFileExistsDelegate,
        IVerifyFilenameDelegate,
        IVerifySizeDelegate,
        IVerifyChunkAsyncDelegate,
        IValidateFileAsyncDelegate<IFileValidationResult>
    {
        // ...
    }
}
