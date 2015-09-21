using System.Collections.Generic;

using GOG.Model;

namespace GOG.Controllers
{
    public class ProductsDataController: 
        CollectionController<ProductData>
    {
        public ProductsDataController(IList<ProductData> productsData): base(productsData)
        {
            // ...
        }
    }
}
