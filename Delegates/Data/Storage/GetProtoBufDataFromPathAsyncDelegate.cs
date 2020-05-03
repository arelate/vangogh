using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage
{
    public abstract class GetProtoBufDataFromPathAsyncDelegate<T> : IGetDataAsyncDelegate<T,string>
    {
        private readonly IGetDataAsyncDelegate<T, string> getProtoBufDataAsyncDelegate;
        private readonly IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate;

        public GetProtoBufDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<T, string> getProtoBufDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate)
        {
            this.getProtoBufDataAsyncDelegate = getProtoBufDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<T> GetDataAsync(string uri = null)
        {
            return await getProtoBufDataAsyncDelegate.GetDataAsync(
                getPathDelegate.GetValue((string.Empty, uri)));
        }
    }
}