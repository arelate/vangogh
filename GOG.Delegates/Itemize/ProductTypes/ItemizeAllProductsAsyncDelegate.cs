using System.Collections.Generic;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Itemizations;

namespace GOG.Delegates.Itemize.ProductTypes
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