using System.Threading.Tasks;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

namespace Delegates.GetData.Storage
{
    public abstract class GetJSONDataFromPathAsyncDelegate<T> : IGetDataAsyncDelegate<T>
    {
        private readonly IGetDataAsyncDelegate<T> getJSONDataAsyncDelegate;
        private readonly IGetPathDelegate getPathDelegate;

        public GetJSONDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<T> getJSONDataAsyncDelegate,
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