using Interfaces.Delegates.Itemize;

using Interfaces.Status;

namespace Delegates.Convert.Collections.Status
{
    public class ConvertStatusTreeToEnumerableDelegate : ConvertTreeToEnumerableDelegate<IStatus>
    {
        public ConvertStatusTreeToEnumerableDelegate(
            IItemizeDelegate<IStatus, IStatus> itemizeStatusChildrenDelegate) :
            base(itemizeStatusChildrenDelegate)
        {
            // ...
        }

    }
}