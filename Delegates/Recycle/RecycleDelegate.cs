using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.File;

using Interfaces.Status;

namespace Delegates.Recycle
{
    public class RecycleAsyncDelegate : IRecycleAsyncDelegate
    {
        private IGetDirectoryAsyncDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDirectoryController directoryController;

        public RecycleAsyncDelegate(
            IGetDirectoryAsyncDelegate getDirectoryDelegate,
            IFileController fileController,
            IDirectoryController directoryController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.directoryController = directoryController;
        }

        public async Task RecycleAsync(string uri, IStatus status)
        {
            var recycleBinUri = Path.Combine(
                await getDirectoryDelegate.GetDirectoryAsync(string.Empty, status),
                uri);
            fileController.Move(uri, recycleBinUri);
        }
    }
}
