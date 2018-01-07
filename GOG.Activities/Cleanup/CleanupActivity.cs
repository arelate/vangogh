using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Directory;

using Interfaces.Status;
using Interfaces.ContextDefinitions;

namespace GOG.Activities.Cleanup
{
    public class CleanupActivity : Activity
    {
        private Context context;
        private IItemizeMultipleAsyncDelegate<string> itemizeMultipleExpectedItemsAsyncDelegate;
        private IItemizeMultipleAsyncDelegate<string> itemizeMultipleActualItemsAsyncDelegate;
        private IItemizeDelegate<string, string> itemizeDetailsDelegate;
        private IFormatDelegate<string, string> formatSupplementaryItemDelegate;
        private IRecycleDelegate moveToRecycleBinDelegate;
        private IDirectoryController directoryController;

        public CleanupActivity(
            Context context,
            IItemizeMultipleAsyncDelegate<string> itemizeMultipleExpectedItemsAsyncDelegate,
            IItemizeMultipleAsyncDelegate<string> itemizeMultipleActualItemsAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate moveToRecycleBinDelegate,
            IDirectoryController directoryController,
            IStatusController statusController) :
            base(statusController)
        {
            this.context = context;
            this.itemizeMultipleExpectedItemsAsyncDelegate = itemizeMultipleExpectedItemsAsyncDelegate;
            this.itemizeMultipleActualItemsAsyncDelegate = itemizeMultipleActualItemsAsyncDelegate;
            this.itemizeDetailsDelegate = itemizeDetailsDelegate;
            this.formatSupplementaryItemDelegate = formatSupplementaryItemDelegate;
            this.moveToRecycleBinDelegate = moveToRecycleBinDelegate;
            this.directoryController = directoryController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var cleanupTask = await statusController.CreateAsync(status, $"Cleanup {context}");

            var expectedItems = await itemizeMultipleExpectedItemsAsyncDelegate.ItemizeMulitpleAsync(status);
            var actualItems = await itemizeMultipleActualItemsAsyncDelegate.ItemizeMulitpleAsync(status);

            var unexpectedItems = actualItems.Except(expectedItems);
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

                moveToRecycleBinDelegate.Recycle(item);
            }

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

            await statusController.CompleteAsync(moveToRecycleBinTask);
        }
    }
}
