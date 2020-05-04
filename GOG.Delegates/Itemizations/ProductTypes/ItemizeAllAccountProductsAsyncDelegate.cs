using System.Collections.Generic;
using Attributes;
using Delegates.Itemizations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Itemizations.ProductTypes
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