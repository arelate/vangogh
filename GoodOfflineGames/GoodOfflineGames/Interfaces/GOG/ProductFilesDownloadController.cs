using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IUpdateProductFilesDelegate {
        Task<IList<ProductFile>> UpdateProductFiles(
        List<DownloadEntry> downloadEntries,
        long id,
        string operatingSystem = "",
        string language = "");
    }

    public interface IUpdateProductOperatingSystemFilesDelegate {
        Task<IList<ProductFile>> UpdateProductOperatingSystemFiles(
            OperatingSystemsDownloads operatingSystemDownloads,
            ICollection<string> downloadOperatingSystems,
            long id);
    }

    public interface IUpdateFilesDelegate
    {
        Task<IList<ProductFile>> UpdateFiles(
            GameDetails details,
            ICollection<string> requiredLanguageCodes,
            ICollection<string> supportedOperatingSystems,
            long context = 0);
    }

    public interface IProductFilesDownloadController:
        IUpdateProductFilesDelegate,
        IUpdateProductOperatingSystemFilesDelegate,
        IUpdateFilesDelegate
    {
        // ...
    }
}
