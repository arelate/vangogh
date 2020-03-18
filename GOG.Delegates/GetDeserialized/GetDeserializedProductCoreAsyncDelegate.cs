using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Network;
using Interfaces.Delegates.Convert;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public abstract class GetDeserializedProductCoreAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
        where T : ProductCore
    {
        readonly IGetResourceAsyncDelegate getResourceAsyncDelegate;
        readonly IConvertDelegate<string, T> convertJSONToProductCoreDelegate;

        public GetDeserializedProductCoreAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IConvertDelegate<string, T> convertJSONToProductCoreDelegate)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.convertJSONToProductCoreDelegate = convertJSONToProductCoreDelegate;
        }

        public async Task<T> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getResourceAsyncDelegate.GetResourceAsync(uri, parameters);

            if (response == null) return default(T);

            return convertJSONToProductCoreDelegate.Convert(response);
        }
    }
}
