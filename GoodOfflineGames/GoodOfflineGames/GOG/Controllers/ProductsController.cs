using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    public class ProductsController: 
        CollectionController<Product>,
        IProductsController<Product>
    {
        public ProductsController(
            IList<Product> products) :
            base(products)
        {
            // ...
        }

        public Product Find(string title)
        {
            return Find(p => p.Title == title);
        }

        public Product Find(long id)
        {
            return Find(p => p.Id == id);
        }

        public IEnumerable<Product> Find(IEnumerable<string> names)
        {
            return Reduce(p => names.Contains(p.Title));
        }
    }
}

