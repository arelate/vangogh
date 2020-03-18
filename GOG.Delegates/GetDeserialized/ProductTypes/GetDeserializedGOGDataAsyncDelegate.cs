using Interfaces.Controllers.Network;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<GOGData>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Network.NetworkController,Controllers",
            "GOG.Delegates.Itemize.ItemizeGOGDataDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToGOGDataDelegate,GOG.Delegates")]
        public GetDeserializedGOGDataAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, GOGData> convertJSONToGOGDataDelegate) :
            base(
                getResourceAsyncDelegate,
                itemizeGogDataDelegate,
                convertJSONToGOGDataDelegate)
        {
            // ...
        }
    }
}