using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations.ProductTypes;
using Delegates.Conversions;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteWishlistedAsyncDelegate : DeleteAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate),
            typeof(ConvertPassthroughIndexDelegate),
            typeof(ConfirmWishlistedContainIdAsyncDelegate))]
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