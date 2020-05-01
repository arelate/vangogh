using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateWishlistedAsyncDelegate: UpdateDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Models.ProductTypes.DeleteWishlistedAsyncDelegate),
            typeof(Delegates.Convert.ConvertPassthroughIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmWishlistedContainIdAsyncDelegate),
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