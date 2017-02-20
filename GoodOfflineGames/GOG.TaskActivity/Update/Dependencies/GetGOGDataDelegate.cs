using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.Network;

namespace GOG.TaskActivities.Update.Dependencies
{
    public class GetGOGDataDelegate<T> : IGetDeserializedDelegate<T>
    {
        private INetworkController networkController;
        private IExtractionController gogDataExtractionController;
        private ISerializationController<string> serializationController;

        public GetGOGDataDelegate(
            INetworkController networkController,
            IExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController)
        {
            this.networkController = networkController;
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
        }

        public async Task<T> GetDeserialized(string uri, IDictionary<string, string> parameters = null)
        {
            var response = await networkController.Get(uri, parameters);

            var dataCollection = gogDataExtractionController.ExtractMultiple(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.First();

            var gogData = serializationController.Deserialize<T>(content);
            return gogData;

            //var gameProductData = gogData.GameProductData;
        }
    }
}
