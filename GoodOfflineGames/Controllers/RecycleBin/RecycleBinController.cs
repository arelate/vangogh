using Interfaces.RecycleBin;
using Interfaces.File;

namespace Controllers.RecycleBin
{
    public class RecycleBinController : IRecycleBinController
    {
        private string recycleBinUri;
        private IFileController fileController;

        public RecycleBinController(
            string recycleBinUri,
            IFileController fileController)
        {
            this.recycleBinUri = recycleBinUri;
        }

        public void MoveToRecycleBin(string uri)
        {
            fileController.Move(uri, recycleBinUri);
        }
    }
}
