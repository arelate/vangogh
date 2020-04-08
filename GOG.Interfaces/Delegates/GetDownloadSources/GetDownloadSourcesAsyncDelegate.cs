using System.Collections.Generic;
using System.Threading.Tasks;


namespace GOG.Interfaces.Delegates.GetDownloadSources
{
    public interface IGetDownloadSourcesAsyncDelegate
    {
        Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync();
    }
}