using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.DownloadSources
{
    public interface IGetDownloadSourcesDelegate
    {
        Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync();
    }

    public interface IDownloadSourcesController :
        IGetDownloadSourcesDelegate
    {
        // ...
    }
}
