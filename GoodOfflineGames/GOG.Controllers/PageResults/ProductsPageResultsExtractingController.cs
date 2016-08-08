using System.Collections.Generic;

using GOG.Interfaces.PageResults;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class ProductsPageResultsExtractingController : IPageResultsExtractingController<ProductsPageResult, Product>
    {
        public IList<Product> Extract(IList<ProductsPageResult> pageResults)
        {
            var products = new List<Product>();

            foreach (var pageResult in pageResults)
                products.AddRange(pageResult.Products);

            return products;
        }
    }
}
