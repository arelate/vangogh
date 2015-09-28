using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface IUpdateDelegate
    {
        Task Update(IConsoleController consoleController = null);
    }

    public interface IProductCoreController<Type>:
        ICollectionController<Type>,
        IFindDelegate<long, Type>,
        IUpdateDelegate
    {
        event EventHandler<Type> OnProductUpdated;
    }
}
