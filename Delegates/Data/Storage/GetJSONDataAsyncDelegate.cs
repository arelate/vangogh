using System.Threading.Tasks;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage
{
    public abstract class GetJSONDataAsyncDelegate<T> : IGetDataAsyncDelegate<T, string>
    {
        private T data;
        private readonly IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;

        public GetJSONDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
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