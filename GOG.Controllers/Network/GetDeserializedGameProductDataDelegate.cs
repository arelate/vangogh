using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Controllers.Network
{
    public class GetGameProductDataDeserializedDelegate: IGetDeserializedAsyncDelegate<GameProductData>
    {
        private IGetDeserializedAsyncDelegate<GOGData> gogDataGetDeserializedDelegate;

        public GetGameProductDataDeserializedDelegate(
            IGetDeserializedAsyncDelegate<GOGData> gogDataGetDeserializedDelegate)
        {
            this.gogDataGetDeserializedDelegate = gogDataGetDeserializedDelegate;
        }

        public async Task<GameProductData> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null)
        {
            var gogData = await gogDataGetDeserializedDelegate.GetDeserializedAsync(status, uri, parameters);
            return gogData.GameProductData;
        }
    }
}
