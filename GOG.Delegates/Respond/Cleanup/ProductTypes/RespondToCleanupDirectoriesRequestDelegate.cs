using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using Delegates.Itemize;
using Delegates.Format.Uri;
using Delegates.Recycle;
using Delegates.Activities;

namespace GOG.Delegates.Respond.Cleanup.ProductTypes
{
    [RespondsToRequests(Method = "cleanup", Collection = "directories")]
    public class RespondToCleanupDirectoriesRequestDelegate : RespondToCleanupRequestDelegate<ProductDirectory>
    {
        [Dependencies(
            typeof(GOG.Delegates.Itemize.ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate),
            typeof(GOG.Delegates.Itemize.ItemizeAllUpdatedProductFilesAsyncDelegate),
            typeof(ItemizePassthroughDelegate),
            typeof(FormatValidationFileDelegate),
            typeof(RecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToCleanupDirectoriesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductDirectoriesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductDirectoriesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllExpectedProductDirectoriesAsyncDelegate,
                itemizeAllActualProductDirectoriesAsyncDelegate,
                itemizeDetailsDelegate,
                formatSupplementaryItemDelegate,
                recycleDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}