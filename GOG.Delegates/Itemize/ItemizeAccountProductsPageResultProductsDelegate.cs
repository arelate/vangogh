using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using GOG.Models;

namespace GOG.Delegates.Itemize
{
    public class ItemizeAccountProductsPageResultProductsDelegate : IItemizeDelegate<IList<AccountProductsPageResult>, AccountProduct>
    {
        public IEnumerable<AccountProduct> Itemize(IList<AccountProductsPageResult> pageResults)
        {
            var products = new List<AccountProduct>();

            foreach (var pageResult in pageResults)
                products.AddRange(pageResult.Products);

            return products;
        }
    }
}
