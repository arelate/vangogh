using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;

using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public class GetDeserializedGOGDataAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
    {
        readonly IGetResourceAsyncDelegate getResourceAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeGogDataDelegate;
        readonly ISerializationController<string> serializationController;

        public GetDeserializedGOGDataAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            ISerializationController<string> serializationController)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.itemizeGogDataDelegate = itemizeGogDataDelegate;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getResourceAsyncDelegate.GetResourceAsync(status, uri, parameters);

            var dataCollection = itemizeGogDataDelegate.Itemize(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.Single();

            var gogData = serializationController.Deserialize<T>(content);
            return gogData;
        }
    }
}
