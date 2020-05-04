using System.Threading.Tasks;
using Attributes;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetDeserializedGameProductDataAsyncDelegate : IGetDataAsyncDelegate<GameProductData, string>
    {
        private readonly IGetDataAsyncDelegate<GOGData, string> gogDataGetDeserializedDelegate;

        [Dependencies(
            typeof(GetDeserializedGOGDataAsyncDelegate))]
        public GetDeserializedGameProductDataAsyncDelegate(
            IGetDataAsyncDelegate<GOGData, string> gogDataGetDeserializedDelegate)
        {
            this.gogDataGetDeserializedDelegate = gogDataGetDeserializedDelegate;
        }

        public async Task<GameProductData> GetDataAsync(string uri)
        {
            var gogData = await gogDataGetDeserializedDelegate.GetDataAsync(uri);
            return gogData.GameProductData;
        }
    }
}