using System.Collections.Generic;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IProductsController:
        IFindController<string, Product>,
        IFindController<long, Product>,
        IFindCollectionController<string, Product>,
        ICollectionController<Product>
    {
        // ...
    }
}
