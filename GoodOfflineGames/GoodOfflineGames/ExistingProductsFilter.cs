using System.Collections.Generic;

using Interfaces.Filtering;
using GOG.Interfaces.Models;

namespace GoodOfflineGames
{
    public class ExistingProductsFilter : IFilterDelegate<IProduct>
    {
        public IEnumerable<IProduct> Filter(IEnumerable<IProduct> products, IList<IProduct> existingProducts)
        {
            foreach (var product in products)
            {
                // filter products in current set = return products that are NOT present in current set
                if (!existingProducts.Contains(product))
                {
                    yield return product;
                }
            }
        }
    }
}
