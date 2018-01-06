using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.MoveToRecycleBin;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.File;

namespace Delegates.MoveToRecycleBin
{
    public class MoveToRecycleBinDelegate : IMoveToRecycleBinDelegate
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDirectoryController directoryController;

        public MoveToRecycleBinDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IFileController fileController,
            IDirectoryController directoryController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.directoryController = directoryController;
        }

        public void MoveToRecycleBin(string uri)
        {
            var recycleBinUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                uri);
            fileController.Move(uri, recycleBinUri);
        }
    }
}
