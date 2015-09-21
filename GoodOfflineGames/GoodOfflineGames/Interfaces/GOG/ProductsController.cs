using System.Collections.Generic;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IProductsController<Type>:
        IFindDelegate<string, Type>,
        IFindDelegate<long, Type>,
        IFindCollectionDelegate<string, Type>,
        ICollectionController<Type>
    {
        // ...
    }
}
