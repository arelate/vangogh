using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using GOG.Models;

namespace GOG.Delegates.Itemize
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