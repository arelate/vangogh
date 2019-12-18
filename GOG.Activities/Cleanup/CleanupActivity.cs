using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Activity;

using Interfaces.Controllers.Logs;
using Interfaces.Controllers.Directory;

using Models.ProductTypes;

namespace GOG.Activities.Cleanup
{
    public abstract class CleanupActivity<Type> : IActivity
        where Type: ProductCore
    {
        readonly IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate;
        readonly IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeDetailsDelegate;
        readonly IFormatDelegate<string, string> formatSupplementaryItemDelegate;
        readonly IRecycleDelegate recycleDelegate;
        readonly IDirectoryController directoryController;
        readonly IResponseLogController responseLogController;

        public CleanupActivity(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IDirectoryController directoryController,
            IResponseLogController responseLogController)
        {
            this.itemizeAllExpectedItemsAsyncDelegate = itemizeAllExpectedItemsAsyncDelegate;
            this.itemizeAllActualItemsAsyncDelegate = itemizeAllActualItemsAsyncDelegate;
            this.itemizeDetailsDelegate = itemizeDetailsDelegate;
            this.formatSupplementaryItemDelegate = formatSupplementaryItemDelegate;
            this.recycleDelegate = recycleDelegate;
            this.directoryController = directoryController;
            this.responseLogController = responseLogController;
        }

        public async Task ProcessActivityAsync()
        {
            responseLogController.OpenResponseLog($"Cleanup {typeof(Type)}");

            var unexpectedItems = new List<string>();
            await foreach (var actualItem in itemizeAllActualItemsAsyncDelegate.ItemizeAllAsync())
                unexpectedItems.Add(actualItem);

            await foreach (var expectedItem in itemizeAllExpectedItemsAsyncDelegate.ItemizeAllAsync())
                unexpectedItems.Remove(expectedItem);

            var cleanupItems = new List<string>();

            foreach (var unexpectedItem in unexpectedItems)
                foreach (var detailedItem in itemizeDetailsDelegate.Itemize(unexpectedItem))
                {
                    cleanupItems.Add(detailedItem);
                    cleanupItems.Add(formatSupplementaryItemDelegate.Format(detailedItem));
                }

            responseLogController.StartAction("Move unexpected items to recycle bin");

            foreach (var item in cleanupItems)
            {
                responseLogController.IncrementActionProgress();
                recycleDelegate.Recycle(item);
            }

            responseLogController.CompleteAction();

            // check if any of the directories are left empty and delete
            var emptyDirectories = new List<string>();
            foreach (var item in cleanupItems)
            {
                var directory = Path.GetDirectoryName(item);
                if (!emptyDirectories.Contains(directory) &&
                    !directoryController.EnumerateFiles(directory).Any()&&
                    !directoryController.EnumerateDirectories(directory).Any())
                    emptyDirectories.Add(directory);
            }

            foreach (var directory in emptyDirectories)
                directoryController.Delete(directory);

            responseLogController.CloseResponseLog();
        }
    }
}
