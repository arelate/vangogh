using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Network;

using Interfaces.Controllers.Serialization;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public abstract class GetDeserializedProductCoreAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
        where T : ProductCore
    {
        readonly IGetResourceAsyncDelegate getResourceAsyncDelegate;
        readonly ISerializationController<string> serializationController;

        public GetDeserializedProductCoreAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            ISerializationController<string> serializationController)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getResourceAsyncDelegate.GetResourceAsync(uri, parameters);

            if (response == null) return default(T);

            return serializationController.Deserialize<T>(response);
        }
    }
}
