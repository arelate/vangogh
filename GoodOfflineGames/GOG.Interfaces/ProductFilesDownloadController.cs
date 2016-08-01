using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Interfaces.Models;

namespace GOG.Interfaces
{
    public interface IUpdateDownloadEntryDelegate
    {
        Task<IProductFile> UpdateDownloadEntry(
            IDownloadEntry downloadEntry,
            long id,
            string operatingSystem = "",
            string language = "");
    }

    public interface IUpdateProductFilesDelegate
    {
        Task<IList<IProductFile>> UpdateProductFiles(
        List<IDownloadEntry> downloadEntries,
        long id,
        string operatingSystem = "",
        string language = "");
    }

    public interface IUpdateProductOperatingSystemFilesDelegate
    {
        Task<IList<IProductFile>> UpdateProductOperatingSystemFiles(
            IOperatingSystemsDownloads operatingSystemDownloads,
            ICollection<string> downloadOperatingSystems,
            long id);
    }

    public interface IUpdateFilesDelegate
    {
        Task<IList<IProductFile>> UpdateFiles(
            IGameDetails details,
            ICollection<string> requiredLanguageCodes,
            ICollection<string> supportedOperatingSystems,
            long context = 0);
    }

    public interface IProductFilesDownloadController :
        IUpdateDownloadEntryDelegate,
        IUpdateProductFilesDelegate,
        IUpdateProductOperatingSystemFilesDelegate,
        IUpdateFilesDelegate
    {
        // ...
    }
}
