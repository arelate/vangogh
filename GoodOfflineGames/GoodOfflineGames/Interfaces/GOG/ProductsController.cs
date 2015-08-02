using System.Collections.Generic;

using GOG.Models;

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
