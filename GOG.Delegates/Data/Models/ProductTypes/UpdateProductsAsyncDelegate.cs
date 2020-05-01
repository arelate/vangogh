using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateProductsAsyncDelegate: UpdateDataAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(DeleteProductsAsyncDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmProductsContainIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate))]
        public UpdateProductsAsyncDelegate(
            IDeleteAsyncDelegate<Product> deleteProductsAsyncDelegate, 
            IConvertDelegate<Product, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmProductsContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<Product>, string> getProductsAsyncDelegate) : 
            base(
                deleteProductsAsyncDelegate, 
                convertProductToIndexDelegate,
                confirmProductsContainsIdAsyncDelegate, 
                getProductsAsyncDelegate)
        {
            // ...
        }
    }
}