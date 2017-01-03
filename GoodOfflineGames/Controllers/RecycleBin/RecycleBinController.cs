using Interfaces.RecycleBin;
using Interfaces.File;
using Interfaces.Directory;

namespace Controllers.RecycleBin
{
    public class RecycleBinController : IRecycleBinController
    {
        private string recycleBinUri;
        private IFileController fileController;
        private IDirectoryController directoryController;

        public RecycleBinController(
            string recycleBinUri,
            IFileController fileController,
            IDirectoryController directoryController)
        {
            this.recycleBinUri = recycleBinUri;
            this.fileController = fileController;
            this.directoryController = directoryController;
        }

        public void MoveFileToRecycleBin(string uri)
        {
            fileController.Move(uri, recycleBinUri);
        }

        public void MoveDirectoryToRecycleBin(string uri)
        {
            directoryController.Move(uri, recycleBinUri);
        }
    }
}
