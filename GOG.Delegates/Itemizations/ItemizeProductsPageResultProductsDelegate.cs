using System.Collections.Generic;
using GOG.Models;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeProductsPageResultProductsDelegate : IItemizeDelegate<IList<ProductsPageResult>, Product>
    {
        public IEnumerable<Product> Itemize(IList<ProductsPageResult> pageResults)
        {
            var products = new List<Product>();

            foreach (var pageResult in pageResults)
                products.AddRange(pageResult.Products);

            return products;
        }
    }
}