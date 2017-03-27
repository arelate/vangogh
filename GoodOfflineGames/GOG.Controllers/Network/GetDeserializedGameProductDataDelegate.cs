using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.Controllers.Network
{
    public class GetGameProductDataDeserializedDelegate: IGetDeserializedDelegate<GameProductData>
    {
        private IGetDeserializedDelegate<GOGData> gogDataGetDeserializedDelegate;

        public GetGameProductDataDeserializedDelegate(
            IGetDeserializedDelegate<GOGData> gogDataGetDeserializedDelegate)
        {
            this.gogDataGetDeserializedDelegate = gogDataGetDeserializedDelegate;
        }

        public async Task<GameProductData> GetDeserialized(ITaskStatus taskStatus, string uri, IDictionary<string, string> parameters = null)
        {
            var gogData = await gogDataGetDeserializedDelegate.GetDeserialized(taskStatus, uri, parameters);
            return gogData.GameProductData;
        }
    }
}
