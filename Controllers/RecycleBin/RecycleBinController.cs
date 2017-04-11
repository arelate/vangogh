using Interfaces.RecycleBin;
using Interfaces.File;
using Interfaces.Directory;
using Interfaces.Destination.Directory;

namespace Controllers.RecycleBin
{
    public class RecycleBinController : IRecycleBinController
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDirectoryController directoryController;

        public RecycleBinController(
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
            fileController.Move(uri, getDirectoryDelegate.GetDirectory());
        }

        //public void MoveDirectoryToRecycleBin(string uri)
        //{
        //    directoryController.Move(uri, getDirectoryDelegate.GetDirectory());
        //}
    }
}
