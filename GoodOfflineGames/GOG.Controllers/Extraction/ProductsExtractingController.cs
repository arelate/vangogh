using System.Collections.Generic;

using GOG.Interfaces.Extraction;

using GOG.Models;

namespace GOG.Controllers.Extraction
{
    public class ProductsExtractionController : IPageResultsExtractionController<ProductsPageResult, Product>
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
