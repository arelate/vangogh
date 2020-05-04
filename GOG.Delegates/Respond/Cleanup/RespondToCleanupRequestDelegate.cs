using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Itemizations;
using Models.ProductTypes;

namespace GOG.Delegates.Respond.Cleanup
{
    public abstract class RespondToCleanupRequestDelegate<Type> : IRespondAsyncDelegate
        where Type : ProductCore
    {
        private readonly IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate;
        private readonly IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate;
        private readonly IItemizeDelegate<string, string> itemizeDetailsDelegate;
        private readonly IFormatDelegate<string, string> formatSupplementaryItemDelegate;
        private readonly IDeleteDelegate<string> deleteDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public RespondToCleanupRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedItemsAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualItemsAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IDeleteDelegate<string> deleteDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllExpectedItemsAsyncDelegate = itemizeAllExpectedItemsAsyncDelegate;
            this.itemizeAllActualItemsAsyncDelegate = itemizeAllActualItemsAsyncDelegate;
            this.itemizeDetailsDelegate = itemizeDetailsDelegate;
            this.formatSupplementaryItemDelegate = formatSupplementaryItemDelegate;
            this.deleteDelegate = deleteDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start($"Cleanup {typeof(Type)}");

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

            startDelegate.Start("Move unexpected items to recycle bin");

            foreach (var item in cleanupItems)
            {
                setProgressDelegate.SetProgress();
                deleteDelegate.Delete(item);
            }

            completeDelegate.Complete();

            // check if any of the directories are left empty and delete
            var emptyDirectories = new List<string>();
            foreach (var item in cleanupItems)
            {
                var directory = Path.GetDirectoryName(item);
                if (!emptyDirectories.Contains(directory) &&
                    !Directory.EnumerateFiles(directory).Any() &&
                    !Directory.EnumerateDirectories(directory).Any())
                    emptyDirectories.Add(directory);
            }

            foreach (var directory in emptyDirectories)
                Directory.Delete(directory);

            completeDelegate.Complete();
        }
    }
}