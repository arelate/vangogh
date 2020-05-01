using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetProductByIdAsyncDelegate: GetDataByIdAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListProductDataFromPathAsyncDelegate),
            typeof(GOG.Delegates.Collections.ProductTypes.FindProductDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertProductToIndexDelegate))]
        public GetProductByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<Product>, string> getListProductsAsyncDelegate, 
            IFindDelegate<Product> findDelegate, 
            IConvertDelegate<Product, long> convertProductToIndexDelegate) : 
            base(
                getListProductsAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}