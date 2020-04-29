using System.Threading.Tasks;
using Interfaces.ValidationResults;

namespace Interfaces.Validation
{
    public interface IValidateFileAsyncDelegate<T>
    {
        Task<T> ValidateFileAsync(string uri, string validationSource);
    }

    public interface IFileValidationController :
        IValidateFileAsyncDelegate<IFileValidationResults>
    {
        // ...
    }
}