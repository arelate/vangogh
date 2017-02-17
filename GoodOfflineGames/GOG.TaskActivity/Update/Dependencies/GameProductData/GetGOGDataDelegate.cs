using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.Network;

namespace GOG.TaskActivities.Update.Dependencies.GameProductData
{
    public class GetGOGDataDelegate : IGetDelegate
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

        public async Task<string> Get(string uri, IDictionary<string, string> parameters = null)
        {
            var response = await networkController.Get(uri, parameters);

            var gogDataCollection = gogDataExtractionController.ExtractMultiple(response);

            if (gogDataCollection == null)
                return null;

            var content = gogDataCollection.First();

            var gogData = serializationController.Deserialize<Models.GOGData>(content);
            if (gogData == null) return string.Empty;

            var gameProductData = gogData.GameProductData;

            return serializationController.Serialize(gameProductData);
        }
    }
}
