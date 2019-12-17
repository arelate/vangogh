using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Directory;

using Interfaces.Status;

using Models.ProductTypes;

namespace GOG.Activities.Cleanup
{
    public abstract class CleanupActivity<Type> : Activity
        where Type: ProductCore
    {
        readonly IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate;
        readonly IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeDetailsDelegate;
        readonly IFormatDelegate<string, string> formatSupplementaryItemDelegate;
        readonly IRecycleDelegate recycleDelegate;
        readonly IDirectoryController directoryController;

        public CleanupActivity(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IDirectoryController directoryController,
            IStatusController statusController) :
            base(statusController)
        {
            this.itemizeAllExpectedItemsAsyncDelegate = itemizeAllExpectedItemsAsyncDelegate;
            this.itemizeAllActualItemsAsyncDelegate = itemizeAllActualItemsAsyncDelegate;
            this.itemizeDetailsDelegate = itemizeDetailsDelegate;
            this.formatSupplementaryItemDelegate = formatSupplementaryItemDelegate;
            this.recycleDelegate = recycleDelegate;
            this.directoryController = directoryController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var cleanupTask = await statusController.CreateAsync(status, $"Cleanup {typeof(Type)}");

            var unexpectedItems = new List<string>();
            await foreach (var actualItem in itemizeAllActualItemsAsyncDelegate.ItemizeAllAsync(status))
                unexpectedItems.Add(actualItem);

            await foreach (var expectedItem in itemizeAllExpectedItemsAsyncDelegate.ItemizeAllAsync(status))
                unexpectedItems.Remove(expectedItem);

            var cleanupItems = new List<string>();

            foreach (var unexpectedItem in unexpectedItems)
                foreach (var detailedItem in itemizeDetailsDelegate.Itemize(unexpectedItem))
                {
                    cleanupItems.Add(detailedItem);
                    cleanupItems.Add(formatSupplementaryItemDelegate.Format(detailedItem));
                }

            var moveToRecycleBinTask = await statusController.CreateAsync(status, "Move unexpected items to recycle bin");
            var current = 0;

            foreach (var item in cleanupItems)
            {
                await statusController.UpdateProgressAsync(
                    moveToRecycleBinTask,
                    ++current,
                    cleanupItems.Count,
                    item);

                recycleDelegate.Recycle(item);
            }

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

            await statusController.CompleteAsync(moveToRecycleBinTask);
        }
    }
}
