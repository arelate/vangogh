using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations.ProductTypes;
using Delegates.Conversions;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateWishlistedAsyncDelegate: UpdateDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(DeleteWishlistedAsyncDelegate),
            typeof(ConvertPassthroughIndexDelegate),
            typeof(ConfirmWishlistedContainIdAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate))]
        public UpdateWishlistedAsyncDelegate(
            IDeleteAsyncDelegate<long> deleteWishlistedAsyncDelegate, 
            IConvertDelegate<long, long> convertWishlistedToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<long>, string> getListWishlistedAsyncDelegate) : 
            base(
                deleteWishlistedAsyncDelegate,
                convertWishlistedToIndexDelegate, 
                confirmDataContainsIdAsyncDelegate, 
                getListWishlistedAsyncDelegate)
        {
            // ...
        }
    }
}