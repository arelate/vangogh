using System.IO;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Move;
using Attributes;
using Delegates.Move.IO;
using Delegates.Values.Directories.ProductTypes;

namespace Delegates.Recycle
{
    public class RecycleDelegate : IRecycleDelegate
    {
        private readonly IGetValueDelegate<string,string> getDirectoryDelegate;
        private readonly IMoveDelegate<string> moveFileDelegate;

        [Dependencies(
            typeof(GetRecycleBinDirectoryDelegate),
            typeof(MoveFileDelegate))]
        public RecycleDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IMoveDelegate<string> moveFileDelegate)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.moveFileDelegate = moveFileDelegate;
        }

        public void Recycle(string uri)
        {
            var recycleBinUri = Path.Combine(
                getDirectoryDelegate.GetValue(string.Empty),
                uri);
            moveFileDelegate.Move(uri, recycleBinUri);
        }
    }
}