using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.Destination.Directory;
using Interfaces.TaskStatus;

namespace Controllers.Enumeration
{
    public class ProductFilesDirectoriesEnumerateDelegate : IEnumerateAsyncDelegate
    {
        private IGetDirectoryDelegate productFilesDirectoryDelegate;
        private IDirectoryController directoryController;
        private ITaskStatusController taskStatusController;

        public ProductFilesDirectoriesEnumerateDelegate(
            IGetDirectoryDelegate productFilesDirectoryDelegate,
            IDirectoryController directoryController,
            ITaskStatusController taskStatusController)
        {
            this.productFilesDirectoryDelegate = productFilesDirectoryDelegate;
            this.directoryController = directoryController;
            this.taskStatusController = taskStatusController;
        }

        public async Task<IList<string>> EnumerateAsync(ITaskStatus taskStatus)
        {
            var enumerateProductFilesDirectoriesTask = taskStatusController.Create(taskStatus, "Enumerate productFiles directories");

            var directories = new List<string>();

            await Task.Run(() =>
            {
                var productFilesDirectory = productFilesDirectoryDelegate.GetDirectory();
                directories.AddRange(directoryController.EnumerateDirectories(productFilesDirectory));
            });

            taskStatusController.Complete(enumerateProductFilesDirectoriesTask);

            return directories;
        }
    }
}
