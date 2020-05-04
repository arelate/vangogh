using System.Collections.Generic;
using Attributes;
using Delegates.Itemizations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Itemizations.ProductTypes
{
    public class ItemizeAllProductsAsyncDelegate : ItemizeAllDataAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate))]
        public ItemizeAllProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>, string> getListProductsAsyncDelegate) :
            base(getListProductsAsyncDelegate)
        {
            // ...
        }
    }
}