using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Status;

using Models.ProductCore;

namespace GOG.Controllers.Network
{
    public class GetDeserializedGOGModelDelegate<T> : IGetDeserializedDelegate<T>
        where T : ProductCore
    {
        private IGetDelegate getDelegate;
        private ISerializationController<string> serializationController;

        public GetDeserializedGOGModelDelegate(
            IGetDelegate getDelegate,
            ISerializationController<string> serializationController)
        {
            this.getDelegate = getDelegate;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserialized(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getDelegate.Get(status, uri, parameters);

            if (response == null) return default(T);

            return serializationController.Deserialize<T>(response);
        }
    }
}
