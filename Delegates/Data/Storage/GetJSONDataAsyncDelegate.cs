using System.Threading.Tasks;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage
{
    public abstract class GetJSONDataAsyncDelegate<T> : IGetDataAsyncDelegate<T>
    {
        private T data;
        private readonly IGetDataAsyncDelegate<string> getStringDataAsyncDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;

        public GetJSONDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, T> convertJSONToTypeDelegate)
        {
            this.getStringDataAsyncDelegate = getStringDataAsyncDelegate;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;
        }

        public async Task<T> GetDataAsync(string uri = null)
        {
            if (data == null)
            {
                var serializedData = await getStringDataAsyncDelegate.GetDataAsync(uri);
                data = convertJSONToTypeDelegate.Convert(serializedData);
            }
            return data;
        }
    }
}