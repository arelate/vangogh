using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Controllers.Network;

using Interfaces.Extraction;
using Interfaces.Controllers.Serialization;
using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public class GetDeserializedGOGDataAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
    {
        private IGetResourceAsyncDelegate getResourceAsyncDelegate;
        private IStringExtractionController gogDataExtractionController;
        private ISerializationController<string> serializationController;

        public GetDeserializedGOGDataAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IStringExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getResourceAsyncDelegate.GetResourceAsync(status, uri, parameters);

            var dataCollection = gogDataExtractionController.ExtractMultiple(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.Single();

            var gogData = serializationController.Deserialize<T>(content);
            return gogData;
        }
    }
}
