using System;
using System.Collections.Generic;
using System.Text;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Interfaces.ActivityContext
{
    public interface IGetSupplementaryDelegate
    {
        IEnumerable<AC> GetSupplementary();
    }

    public interface ISupplementaryController:
        IGetSupplementaryDelegate
    {
        // ...
    }
}
