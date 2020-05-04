using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Data.Models
{
    public abstract class GetDeserializedDataAsyncDelegate<T> : IGetDataAsyncDelegate<T, string>
    {
        private readonly IConvertDelegate<(string, IDictionary<string, string>), string>
            convertUriParametersToUriDelegate;

        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;
        private readonly IItemizeDelegate<string, string> itemizeGogDataDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;

        public GetDeserializedDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, T> convertJSONToTypeDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.itemizeGogDataDelegate = itemizeGogDataDelegate;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;
        }

        public async Task<T> GetDataAsync(string uri)
        {
            var response = await getUriDataAsyncDelegate.GetDataAsync(uri);

            var dataCollection = itemizeGogDataDelegate.Itemize(response);

            if (dataCollection == null)
                return default(T);

            var content = dataCollection.Single();

            var gogData = convertJSONToTypeDelegate.Convert(content);
            return gogData;
        }
    }
}