using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Network;

using Interfaces.Serialization;
using Interfaces.Status;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public class GetDeserializedGOGModelAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
        where T : ProductCore
    {
        private IGetResourceAsyncDelegate getResourceAsyncDelegate;
        private ISerializationController<string> serializationController;

        public GetDeserializedGOGModelAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            ISerializationController<string> serializationController)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getResourceAsyncDelegate.GetResourceAsync(status, uri, parameters);

            if (response == null) return default(T);

            return serializationController.Deserialize<T>(response);
        }
    }
}
