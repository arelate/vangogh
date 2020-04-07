using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage
{
    public abstract class GetProtoBufDataFromPathAsyncDelegate<T> : IGetDataAsyncDelegate<T>
    {
        private readonly IGetDataAsyncDelegate<T> getProtoBufDataAsyncDelegate;
        private readonly IGetPathDelegate getPathDelegate;

        public GetProtoBufDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<T> getProtoBufDataAsyncDelegate,
            IGetPathDelegate getPathDelegate)
        {
            this.getProtoBufDataAsyncDelegate = getProtoBufDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<T> GetDataAsync(string uri = null)
        {
            return await getProtoBufDataAsyncDelegate.GetDataAsync(
                getPathDelegate.GetPath(string.Empty, uri));
        }
    }
}