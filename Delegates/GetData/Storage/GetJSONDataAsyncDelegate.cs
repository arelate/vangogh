using System.Threading.Tasks;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Convert;

namespace Delegates.GetData.Storage
{
    public abstract class GetJSONDataAsyncDelegate<T> : IGetDataAsyncDelegate<T>
    {
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
            var serializedData = await getStringDataAsyncDelegate.GetDataAsync(uri);
            return convertJSONToTypeDelegate.Convert(serializedData);
        }
    }
}