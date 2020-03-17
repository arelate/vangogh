using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Move;

using Interfaces.Models.Dependencies;

using Attributes;

namespace Delegates.Recycle
{
    public class RecycleDelegate : IRecycleDelegate
    {
        readonly IGetDirectoryDelegate getDirectoryDelegate;
        readonly IMoveDelegate<string> moveFileDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetDirectory.ProductTypes.GetRecycleBinDirectoryDelegate,Delegates",
            "Delegates.Move.IO.MoveFileDelegate,Delegates")]
        public RecycleDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IMoveDelegate<string> moveFileDelegate)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.moveFileDelegate = moveFileDelegate;
        }

        public void Recycle(string uri)
        {
            var recycleBinUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(string.Empty),
                uri);
            moveFileDelegate.Move(uri, recycleBinUri);
        }
    }
}
