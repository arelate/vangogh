using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetUpdatedByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindUpdatedDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates")]
        public GetUpdatedByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getDataCollectionAsyncDelegate, 
            IFindDelegate<long> findDelegate, 
            IConvertDelegate<long, long> convertProductToIndexDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}