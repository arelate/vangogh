using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.File;

namespace Delegates.Recycle
{
    public class RecycleDelegate : IRecycleDelegate
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDirectoryController directoryController;

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
