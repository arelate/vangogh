using System.Threading.Tasks;

namespace Interfaces.Validation
{
    public interface IDownloadValidationDataDelegate
    {
        Task DownloadValidationData(string resolvedUri);
    }

    public interface IValidationDataDownloadController:
        IDownloadValidationDataDelegate
    {
        // ...
    }
}
