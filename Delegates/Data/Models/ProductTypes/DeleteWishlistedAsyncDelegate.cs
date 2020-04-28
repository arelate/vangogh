using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteWishlistedAsyncDelegate : DeleteAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmWishlistedContainIdAsyncDelegate,Delegates")]
        public DeleteWishlistedAsyncDelegate(
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