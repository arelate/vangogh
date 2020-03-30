using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.GetDeserialized
{
    public abstract class GetDeserializedDataAsyncDelegate<T> : IGetDeserializedAsyncDelegate<T>
    {
        private readonly IConvertDelegate<(string, IDictionary<string,string>), string> convertUriParametersToUriDelegate;
        private readonly IGetDataAsyncDelegate<string> getUriDataAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeGogDataDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;

        public GetDeserializedDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string,string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, T> convertJSONToTypeDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.itemizeGogDataDelegate = itemizeGogDataDelegate;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;
        }

        public async Task<T> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            var uriParameters = convertUriParametersToUriDelegate.Convert((uri, parameters));
            var response = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

            var dataCollection = itemizeGogDataDelegate.Itemize(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.Single();

            var gogData = convertJSONToTypeDelegate.Convert(content);
            return gogData;
        }
    }
}
