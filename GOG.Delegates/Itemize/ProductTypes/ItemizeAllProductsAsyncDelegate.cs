using System.Collections.Generic;
using Delegates.Itemize;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeAllProductsAsyncDelegate : ItemizeAllDataAsyncDelegate<Product>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate,GOG.Delegates")]
        public ItemizeAllProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>, string> getListProductsAsyncDelegate) :
            base(getListProductsAsyncDelegate)
        {
            // ...
        }
    }
}