using System.Collections.Generic;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Itemizations;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeAllAccountProductsAsyncDelegate : ItemizeAllDataAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate))]
        public ItemizeAllAccountProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>, string> getListAccountProductsAsyncDelegate) :
            base(getListAccountProductsAsyncDelegate)
        {
            // ...
        }
    }
}