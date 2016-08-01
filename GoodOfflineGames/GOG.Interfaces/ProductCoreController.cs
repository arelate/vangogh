using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Collection;

namespace GOG.Interfaces
{
    public delegate void BeforeAddingDelegate<Type>(ref Type data, string item);

    public interface IUpdateDelegate<Type>
    {
        Task<IList<Type>> Update(IList<string> items, IReportUpdateDelegate reportUpdateDelegate = null);
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
