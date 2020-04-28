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
            "Delegates.Data.Models.ProductTypes.DeleteUpdatedAsyncDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmUpdatedContainIdAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate,Delegates")]
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