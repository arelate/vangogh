using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage
{
    public abstract class GetJSONDataFromPathAsyncDelegate<T> : IGetDataAsyncDelegate<T, string>
    {
        private readonly IGetDataAsyncDelegate<T, string> getJSONDataAsyncDelegate;
        private readonly IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate;

        public GetJSONDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<T, string> getJSONDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate)
        {
            this.getJSONDataAsyncDelegate = getJSONDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<T> GetDataAsync(string uri = null)
        {
            return await getJSONDataAsyncDelegate.GetDataAsync(
                getPathDelegate.GetValue((string.Empty, uri)));
        }
    }
}