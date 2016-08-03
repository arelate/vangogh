using System.Collections.Generic;

using Interfaces.Filtering;
using GOG.Models;

namespace GoodOfflineGames
{
    public class ExistingProductsFilter : IFilterDelegate<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> products, IList<Product> existingProducts)
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
