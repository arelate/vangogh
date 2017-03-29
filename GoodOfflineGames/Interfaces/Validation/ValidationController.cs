using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Validation
{
    public interface IVerifyFilenameDelegate
    {
        void VerifyFilename(string uri, string expectedFilename);
    }

    public interface IVerifySizeDelegate
    {
        void VerifySize(string uri, long expectedSize);
    }

    public interface IVerifyChunkDelegate
    {
        Task VerifyChunkAsync(System.IO.Stream fileStream, long from, long to, string expectedMd5);
    }

    public interface IValidateDelegate
    {
        Task ValidateAsync(string uri, string validationUri, IStatus status);
    }

    public interface IValidationController:
        IVerifyFilenameDelegate,
        IVerifySizeDelegate,
        IVerifyChunkDelegate,
        IValidateDelegate
    {
        // ...
    }
}
