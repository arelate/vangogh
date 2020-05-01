using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage
{
    public abstract class GetJSONDataFromPathAsyncDelegate<T> : IGetDataAsyncDelegate<T, string>
    {
        private readonly IGetDataAsyncDelegate<T, string> getJSONDataAsyncDelegate;
        private readonly IGetPathDelegate getPathDelegate;

        public GetJSONDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<T, string> getJSONDataAsyncDelegate,
            IGetPathDelegate getPathDelegate)
        {
            this.getJSONDataAsyncDelegate = getJSONDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<T> GetDataAsync(string uri = null)
        {
            return await getJSONDataAsyncDelegate.GetDataAsync(
                getPathDelegate.GetPath(string.Empty, uri));
        }
    }
}