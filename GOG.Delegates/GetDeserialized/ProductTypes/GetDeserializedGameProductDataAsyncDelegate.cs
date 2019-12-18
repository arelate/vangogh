using System.Collections.Generic;
using System.Threading.Tasks;



using GOG.Interfaces.Delegates.GetDeserialized;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedGameProductDataAsyncDelegate : IGetDeserializedAsyncDelegate<GameProductData>
    {
        readonly IGetDeserializedAsyncDelegate<GOGData> gogDataGetDeserializedDelegate;

        [Dependencies("GOG.Delegates.GetDeserialized.ProductTypes.GetDeserializedGOGDataAsyncDelegate,GOG.Delegates")]
        public GetDeserializedGameProductDataAsyncDelegate(
            IGetDeserializedAsyncDelegate<GOGData> gogDataGetDeserializedDelegate)
        {
            this.gogDataGetDeserializedDelegate = gogDataGetDeserializedDelegate;
        }

        public async Task<GameProductData> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            var gogData = await gogDataGetDeserializedDelegate.GetDeserializedAsync(uri, parameters);
            return gogData.GameProductData;
        }
    }
}
