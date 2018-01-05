using System.Collections.Generic;

using GOG.Interfaces.Delegates.ExtractPageResults;

using GOG.Models;

namespace GOG.Delegates.ExtractPageResults
{
    public class ExtractProductsDelegate : IExtractPageResultsDelegate<ProductsPageResult, Product>
    {
        public IEnumerable<Product> ExtractMultiple(IList<ProductsPageResult> pageResults)
        {
            var products = new List<Product>();

            foreach (var pageResult in pageResults)
                products.AddRange(pageResult.Products);

            return products;
        }
    }
}
