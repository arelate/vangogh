using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Interfaces.TaskStatus;

using Interfaces.Enumeration;
using Interfaces.Directory;
using Interfaces.RecycleBin;

namespace GOG.TaskActivities.Cleanup
{
    public class CleanupController : TaskActivityController
    {
        private IEnumerateAsyncDelegate expectedItemsEnumarateDelegate;
        private IEnumerateDelegate actualItemsEnumerateDelegate;
        private IEnumerateDelegate<string> itemsDetailsEnumerateDelegate;
        private IEnumerateDelegate<string> supplementaryItemsEnumerateDelegate;
        private IRecycleBinController recycleBinController;
        private IDirectoryController directoryController;

        public CleanupController(
            IEnumerateAsyncDelegate expectedItemsEnumarateDelegate,
            IEnumerateDelegate actualItemsEnumerateDelegate,
            IEnumerateDelegate<string> itemsDetailsEnumerateDelegate,
            IEnumerateDelegate<string> supplementaryItemsEnumerateDelegate,
            IRecycleBinController recycleBinController,
            IDirectoryController directoryController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.expectedItemsEnumarateDelegate = expectedItemsEnumarateDelegate;
            this.actualItemsEnumerateDelegate = actualItemsEnumerateDelegate;
            this.itemsDetailsEnumerateDelegate = itemsDetailsEnumerateDelegate;
            this.supplementaryItemsEnumerateDelegate = supplementaryItemsEnumerateDelegate;
            this.recycleBinController = recycleBinController;
            this.directoryController = directoryController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var expectedItems = await expectedItemsEnumarateDelegate.EnumerateAsync(taskStatus);
            var actualItems = actualItemsEnumerateDelegate.Enumerate(taskStatus);

            var unexpectedItems = actualItems.Except(expectedItems);
            var cleanupItems = new List<string>();

            foreach (var unexpectedItem in unexpectedItems)
                foreach (var detailedItem in itemsDetailsEnumerateDelegate.Enumerate(unexpectedItem))
                {
                    cleanupItems.Add(detailedItem);
                    cleanupItems.AddRange(supplementaryItemsEnumerateDelegate.Enumerate(detailedItem));
                }

            var moveToRecycleBinTask = taskStatusController.Create(taskStatus, "Move unexpected items to recycle bin");

            foreach (var item in cleanupItems)
                recycleBinController.MoveToRecycleBin(item);

            // check if any of the directories are left empty and delete
            var emptyDirectories = new List<string>();
            foreach (var item in cleanupItems)
            {
                var directory = Path.GetDirectoryName(item);
                if (!emptyDirectories.Contains(directory) &&
                    directoryController.EnumerateFiles(directory).Count() == 0 &&
                    directoryController.EnumerateDirectories(directory).Count() == 0)
                    emptyDirectories.Add(directory);
            }

            foreach (var directory in emptyDirectories)
                directoryController.Delete(directory);

            taskStatusController.Complete(moveToRecycleBinTask);
        }
    }
}
