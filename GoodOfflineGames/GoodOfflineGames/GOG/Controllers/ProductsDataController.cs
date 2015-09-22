using System.Collections.Generic;

using GOG.Model;

namespace GOG.Controllers
{
    public class ProductsDataController: 
        CollectionController<ProductData>
    {
        public ProductsDataController(IList<ProductData> productsData) : base(productsData)
        {
            // ...
        }

        public ProductData Find(long id)
        {
            return Find(pd => pd.Id == id);
        }
    }
}
