using System.IO;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Values.Directories.ProductTypes;

namespace Delegates.Data.Storage
{
    public class DeleteToRecycleDelegate : IDeleteDelegate<string>
    {
        private readonly IGetValueDelegate<string,string> getDirectoryDelegate;
        private readonly IMoveDelegate<string> moveFileDelegate;

        [Dependencies(
            typeof(GetRecycleBinDirectoryDelegate),
            typeof(MoveFileDelegate))]
        public DeleteToRecycleDelegate(
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IMoveDelegate<string> moveFileDelegate)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.moveFileDelegate = moveFileDelegate;
        }

        public void Delete(string uri)
        {
            var recycleBinUri = Path.Combine(
                getDirectoryDelegate.GetValue(string.Empty),
                uri);
            moveFileDelegate.Move(uri, recycleBinUri);
        }
    }
}