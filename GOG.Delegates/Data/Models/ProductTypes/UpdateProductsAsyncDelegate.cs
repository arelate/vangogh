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
            "GOG.Delegates.Data.Models.ProductTypes.DeleteProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmProductsContainIdAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate,GOG.Delegates")]
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