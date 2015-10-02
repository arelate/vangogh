using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public delegate void BeforeAddingDelegate<Type>(ref Type data, string item);

    public interface IUpdateDelegate<Type>
    {
        Task Update(IList<string> items, IPostUpdateDelegate postUpdateDelegate = null);
    }

    public interface IProductCoreController<Type>:
        ICollectionController<Type>,
        IFindDelegate<long, Type>,
        IUpdateDelegate<Type>
    {
        event EventHandler<Type> OnProductUpdated;
        event BeforeAddingDelegate<Type> OnBeforeAdding;
    }
}
