using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Network;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public abstract class GetDeserializedDataAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
    {
        readonly IGetResourceAsyncDelegate getResourceAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeGogDataDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;

        public GetDeserializedDataAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, T> convertJSONToTypeDelegate)
        {
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.itemizeGogDataDelegate = itemizeGogDataDelegate;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;
        }

        public async Task<T> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            var response = await getResourceAsyncDelegate.GetResourceAsync(uri, parameters);

            var dataCollection = itemizeGogDataDelegate.Itemize(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.Single();

            var gogData = convertJSONToTypeDelegate.Convert(content);
            return gogData;
        }
    }
}
