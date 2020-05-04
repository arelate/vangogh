using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations.ProductTypes;
using Delegates.Conversions;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteUpdatedAsyncDelegate : DeleteAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate),
            typeof(ConvertPassthroughIndexDelegate),
            typeof(ConfirmUpdatedContainIdAsyncDelegate))]
        public DeleteUpdatedAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getDataCollectionAsyncDelegate,
            IConvertDelegate<long, long> convertProductToIndexDelegate,
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate) :
            base(
                getDataCollectionAsyncDelegate,
                convertProductToIndexDelegate,
                confirmContainsAsyncDelegate)
        {
            // ...
        }
    }
}