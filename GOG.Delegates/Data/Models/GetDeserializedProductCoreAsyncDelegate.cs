using System.Threading.Tasks;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace GOG.Delegates.Data.Models
{
    public abstract class GetDeserializedProductCoreAsyncDelegate<T> : IGetDataAsyncDelegate<T, string>
        where T : ProductCore
    {
        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;

        private readonly IConvertDelegate<string, T> convertJSONToProductCoreDelegate;

        public GetDeserializedProductCoreAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IConvertDelegate<string, T> convertJSONToProductCoreDelegate)
        {
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.convertJSONToProductCoreDelegate = convertJSONToProductCoreDelegate;
        }

        public async Task<T> GetDataAsync(string uri)
        {
            var response = await getUriDataAsyncDelegate.GetDataAsync(uri);

            if (response == null) return default(T);

            return convertJSONToProductCoreDelegate.Convert(response);
        }
    }
}