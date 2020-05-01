using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteWishlistedAsyncDelegate : DeleteAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate),
            typeof(Convert.ConvertPassthroughIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmWishlistedContainIdAsyncDelegate))]
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