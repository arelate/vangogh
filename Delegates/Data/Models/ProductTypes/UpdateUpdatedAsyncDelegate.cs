using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations.ProductTypes;
using Delegates.Conversions;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateUpdatedAsyncDelegate: UpdateDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(DeleteUpdatedAsyncDelegate),
            typeof(ConvertPassthroughIndexDelegate),
            typeof(ConfirmUpdatedContainIdAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate))]
        public UpdateUpdatedAsyncDelegate(
            IDeleteAsyncDelegate<long> deleteUpdatedAsyncDelegate, 
            IConvertDelegate<long, long> convertUpdatedToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<long>, string> getListUpdatedAsyncDelegate) : 
            base(
                deleteUpdatedAsyncDelegate,
                convertUpdatedToIndexDelegate, 
                confirmDataContainsIdAsyncDelegate, 
                getListUpdatedAsyncDelegate)
        {
            // ...
        }
    }
}