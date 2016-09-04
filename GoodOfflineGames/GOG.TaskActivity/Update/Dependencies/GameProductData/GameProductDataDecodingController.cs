using System.Linq;

using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.UpdateDependencies;

namespace GOG.TaskActivities.Update.Dependencies.GameProductData
{
    public class GameProductDataDecodingController : IDataDecodingController
    {
        IExtractionController gogDataExtractionController;
        ISerializationController<string> serializationController;

        public GameProductDataDecodingController(
            IExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController)
        {
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
        }

        public string DecodeData(string data)
        {
            var gogDataCollection = gogDataExtractionController.ExtractMultiple(data);

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
