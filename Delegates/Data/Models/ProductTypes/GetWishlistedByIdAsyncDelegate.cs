using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetWishlistedByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindWishlistedDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates")]
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