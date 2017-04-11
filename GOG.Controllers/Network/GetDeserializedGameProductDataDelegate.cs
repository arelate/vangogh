using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Status;

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

        public async Task<GameProductData> GetDeserialized(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var gogData = await gogDataGetDeserializedDelegate.GetDeserialized(status, uri, parameters);
            return gogData.GameProductData;
        }
    }
}
