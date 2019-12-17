using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.File;

using Attributes;

namespace Delegates.Recycle
{
    public class RecycleDelegate : IRecycleDelegate
    {
        readonly IGetDirectoryDelegate getDirectoryDelegate;
        readonly IFileController fileController;
        IDirectoryController directoryController;

        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetRecycleBinDirectoryDelegate,Delegates",
            "Controllers.File.FileController,Controllers",
            "Controllers.Directory.DirectoryController,Controllers")]
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
