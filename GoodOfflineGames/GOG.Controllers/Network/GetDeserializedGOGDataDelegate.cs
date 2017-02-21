using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.Network;

namespace GOG.Controllers.Network
{
    public class GetDeserializedGOGDataDelegate<T> : IGetDeserializedDelegate<T>
    {
        private IGetDelegate getDelegate;
        private IExtractionController gogDataExtractionController;
        private ISerializationController<string> serializationController;

        public GetDeserializedGOGDataDelegate(
            IGetDelegate getDelegate,
            IExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController)
        {
            this.getDelegate = getDelegate;
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserialized(string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getDelegate.Get(uri, parameters);

            var dataCollection = gogDataExtractionController.ExtractMultiple(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.First();

            var gogData = serializationController.Deserialize<T>(content);
            return gogData;
        }
    }
}
