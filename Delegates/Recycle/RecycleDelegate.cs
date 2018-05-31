using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.File;

namespace Delegates.Recycle
{
    public class RecycleDelegate : IRecycleDelegate
    {
        readonly IGetDirectoryDelegate getDirectoryDelegate;
        readonly IFileController fileController;
        IDirectoryController directoryController;

        public RecycleDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IFileController fileController,
            IDirectoryController directoryController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.directoryController = directoryController;
        }

        public void Recycle(string uri)
        {
            var recycleBinUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(string.Empty),
                uri);
            fileController.Move(uri, recycleBinUri);
        }
    }
}
