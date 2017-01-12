using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace Interfaces.DownloadSources
{
    public interface IGetDownloadSourcesDelegate
    {
        Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus);
    }

    public interface IDownloadSourcesController :
        IGetDownloadSourcesDelegate
    {
        // ...
    }
}
