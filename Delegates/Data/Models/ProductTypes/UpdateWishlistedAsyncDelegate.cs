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
            "Delegates.Data.Models.ProductTypes.DeleteWishlistedAsyncDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmWishlistedContainIdAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate,Delegates")]
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