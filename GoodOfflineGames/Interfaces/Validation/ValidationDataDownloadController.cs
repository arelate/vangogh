using System.Threading.Tasks;

namespace Interfaces.Validation
{
    public interface IDownloadValidationDataDelegate
    {
        Task DownloadValidationDataAsync(string resolvedUri);
    }

    public interface IValidationDataDownloadController:
        IDownloadValidationDataDelegate
    {
        // ...
    }
}
