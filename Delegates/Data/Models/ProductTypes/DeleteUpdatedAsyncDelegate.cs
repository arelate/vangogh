using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteUpdatedAsyncDelegate : DeleteAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate),
            typeof(Delegates.Convert.ConvertPassthroughIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmUpdatedContainIdAsyncDelegate))]
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