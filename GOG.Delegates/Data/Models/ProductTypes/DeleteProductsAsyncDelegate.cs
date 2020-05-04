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
    public class DeleteProductsAsyncDelegate: DeleteAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate),
            typeof(ConvertProductToIndexDelegate),
            typeof(ConfirmProductsContainIdAsyncDelegate))]
        public DeleteProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>, string> getDataCollectionAsyncDelegate, 
            IConvertDelegate<Product, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                convertProductToIndexDelegate, 
                confirmContainsAsyncDelegate)
        {
            // ...
        }
    }
}