using System.Collections.Generic;
using Delegates.Itemize;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeAllAccountProductsAsyncDelegate : ItemizeAllDataAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates")]
        public ItemizeAllAccountProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>, string> getListAccountProductsAsyncDelegate) :
            base(getListAccountProductsAsyncDelegate)
        {
            // ...
        }
    }
}