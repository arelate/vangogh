using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateUpdatedAsyncDelegate: UpdateDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Models.ProductTypes.DeleteUpdatedAsyncDelegate),
            typeof(Delegates.Convert.ConvertPassthroughIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmUpdatedContainIdAsyncDelegate),
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