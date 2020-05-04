using System.Collections.Generic;
using GOG.Models;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations
{
    public class
        ItemizeAccountProductsPageResultProductsDelegate : IItemizeDelegate<IList<AccountProductsPageResult>,
            AccountProduct>
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