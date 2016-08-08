using System.Collections.Generic;

using GOG.Interfaces.PageResults;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class AccountProductsPageResultsExtractingController : IPageResultsExtractingController<AccountProductsPageResult, AccountProduct>
    {
        public IList<AccountProduct> Extract(IList<AccountProductsPageResult> pageResults)
        {
            var products = new List<AccountProduct>();

            foreach (var pageResult in pageResults)
                products.AddRange(pageResult.AccountProducts);

            return products;
        }
    }
}
