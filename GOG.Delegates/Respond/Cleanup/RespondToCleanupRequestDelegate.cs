using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Logs;

using Models.ProductTypes;

namespace GOG.Delegates.Respond.Cleanup
{
    public abstract class RespondToCleanupRequestDelegate<Type> : IRespondAsyncDelegate
        where Type: ProductCore
    {
        readonly IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate;
        readonly IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate;
        readonly IItemizeDelegate<string, string> itemizeDetailsDelegate;
        readonly IFormatDelegate<string, string> formatSupplementaryItemDelegate;
        readonly IRecycleDelegate recycleDelegate;
        readonly IActionLogController actionLogController;

        public RespondToCleanupRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IActionLogController actionLogController)
        {
            this.itemizeAllExpectedItemsAsyncDelegate = itemizeAllExpectedItemsAsyncDelegate;
            this.itemizeAllActualItemsAsyncDelegate = itemizeAllActualItemsAsyncDelegate;
            this.itemizeDetailsDelegate = itemizeDetailsDelegate;
            this.formatSupplementaryItemDelegate = formatSupplementaryItemDelegate;
            this.recycleDelegate = recycleDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            actionLogController.StartAction($"Cleanup {typeof(Type)}");

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

            actionLogController.StartAction("Move unexpected items to recycle bin");

            foreach (var item in cleanupItems)
            {
                actionLogController.IncrementActionProgress();
                recycleDelegate.Recycle(item);
            }

            actionLogController.CompleteAction();

            // check if any of the directories are left empty and delete
            var emptyDirectories = new List<string>();
            foreach (var item in cleanupItems)
            {
                var directory = Path.GetDirectoryName(item);
                if (!emptyDirectories.Contains(directory) &&
                    !Directory.EnumerateFiles(directory).Any()&&
                    !Directory.EnumerateDirectories(directory).Any())
                    emptyDirectories.Add(directory);
            }

            foreach (var directory in emptyDirectories)
                Directory.Delete(directory);

            actionLogController.CompleteAction();
        }
    }
}
