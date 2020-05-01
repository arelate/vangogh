using System.IO;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Move;
using Attributes;
using Delegates.GetDirectory.ProductTypes;
using Delegates.Move.IO;

namespace Delegates.Recycle
{
    public class RecycleDelegate : IRecycleDelegate
    {
        private readonly IGetDirectoryDelegate getDirectoryDelegate;
        private readonly IMoveDelegate<string> moveFileDelegate;

        [Dependencies(
            typeof(GetRecycleBinDirectoryDelegate),
            typeof(MoveFileDelegate))]
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