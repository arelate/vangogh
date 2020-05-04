using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateProductsAsyncDelegate: UpdateDataAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(DeleteProductsAsyncDelegate),
            typeof(ConvertProductToIndexDelegate),
            typeof(ConfirmProductsContainIdAsyncDelegate),
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