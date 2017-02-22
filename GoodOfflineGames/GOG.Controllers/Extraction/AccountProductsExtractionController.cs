using System.Collections.Generic;

using GOG.Interfaces.Extraction;

using GOG.Models;

namespace GOG.Controllers.Extraction
{
    public class AccountProductsExtractionController : IPageResultsExtractionController<AccountProductsPageResult, AccountProduct>
    {
        public IEnumerable<AccountProduct> ExtractMultiple(IList<AccountProductsPageResult> pageResults)
        {
            var products = new List<AccountProduct>();

            foreach (var pageResult in pageResults)
                products.AddRange(pageResult.Products);

            return products;
        }
    }
}
