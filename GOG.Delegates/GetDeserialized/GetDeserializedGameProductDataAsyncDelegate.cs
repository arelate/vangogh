using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

using GOG.Interfaces.Delegates.GetDeserialized;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized
{
    public class GetDeserializedGameProductDataAsyncDelegate : IGetDeserializedAsyncDelegate<GameProductData>
    {
        readonly IGetDeserializedAsyncDelegate<GOGData> gogDataGetDeserializedDelegate;

        [Dependencies("GOG.Delegates.GetDeserialized.ProductTypes.GetGOGDataDeserializedGOGDataAsyncDelegate,GOG.Delegates")]
        public GetDeserializedGameProductDataAsyncDelegate(
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
