using Interfaces.Delegates.Itemize;

using Interfaces.Status;

using Attributes;

namespace Delegates.Convert.Status
{
    public class ConvertStatusTreeToEnumerableDelegate : ConvertTreeToEnumerableDelegate<IStatus>
    {
        [Dependencies("Delegates.Itemize.ItemizeStatusChildrenDelegate,Delegates")]
        public ConvertStatusTreeToEnumerableDelegate(
            IItemizeDelegate<IStatus, IStatus> itemizeStatusChildrenDelegate) :
            base(itemizeStatusChildrenDelegate)
        {
            // ...
        }

    }
}