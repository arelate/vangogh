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
            typeof(Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindUpdatedDelegate),
            typeof(Delegates.Convert.ConvertPassthroughIndexDelegate))]
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