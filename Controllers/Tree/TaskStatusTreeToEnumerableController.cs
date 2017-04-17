using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Tree;
using Interfaces.Status;

namespace Controllers.Tree
{
    public class StatusTreeToEnumerableController : TreeToEnumerableController<IStatus>
    {
        public override IEnumerable<IStatus> GetChildren(IStatus item)
        {
            return item != null ? item.Children : null;
        }
    }
}
