using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Interfaces.Status;

namespace Delegates.Itemize
{
    public class ItemizeStatusChildrenDelegate : IItemizeDelegate<IStatus, IStatus>
    {
        public IEnumerable<IStatus> Itemize(IStatus item)
        {
            return item?.Children;
        }
    }
}
