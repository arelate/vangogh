using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Status;

using Models.ProductCore;

namespace GOG.Controllers.Network
{
    public class GetDeserializedGOGModelDelegate<T> : IGetDeserializedAsyncDelegate<T>
        where T : ProductCore
    {
        private IGetAsyncDelegate getDelegate;
        private ISerializationController<string> serializationController;

        public GetDeserializedGOGModelDelegate(
            IGetAsyncDelegate getDelegate,
            ISerializationController<string> serializationController)
        {
            this.getDelegate = getDelegate;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getDelegate.GetAsync(status, uri, parameters);

            if (response == null) return default(T);

            return serializationController.Deserialize<T>(response);
        }
    }
}
