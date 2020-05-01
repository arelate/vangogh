using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetWishlistedByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindWishlistedDelegate),
            typeof(Convert.ConvertPassthroughIndexDelegate))]
        public GetWishlistedByIdAsyncDelegate(
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