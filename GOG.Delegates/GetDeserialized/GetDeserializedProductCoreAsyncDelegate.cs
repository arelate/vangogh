using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public abstract class GetDeserializedProductCoreAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
        where T : ProductCore
    {
        private readonly IGetDataAsyncDelegate<string> getUriDataAsyncDelegate;
        private readonly IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate;
        readonly IConvertDelegate<string, T> convertJSONToProductCoreDelegate;

        public GetDeserializedProductCoreAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,            
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
            IConvertDelegate<string, T> convertJSONToProductCoreDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.convertJSONToProductCoreDelegate = convertJSONToProductCoreDelegate;
        }

        public async Task<T> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            var uriParameters = convertUriParametersToUriDelegate.Convert((uri, parameters));
            var response = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

            if (response == null) return default(T);

            return convertJSONToProductCoreDelegate.Convert(response);
        }
    }
}
