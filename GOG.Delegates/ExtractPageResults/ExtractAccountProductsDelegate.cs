using System.Collections.Generic;

using GOG.Interfaces.Delegates.ExtractPageResults;

using GOG.Models;

namespace GOG.Delegates.ExtractPageResults
{
    public class ExtractAccountProductsDelegate : IExtractPageResultsDelegate<AccountProductsPageResult, AccountProduct>
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
