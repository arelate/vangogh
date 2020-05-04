using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using Delegates.Itemize;
using Delegates.Format.Uri;
using Delegates.Data.Storage;
using Delegates.Activities;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Respond.Cleanup.ProductTypes
{
    [RespondsToRequests(Method = "cleanup", Collection = "directories")]
    public class RespondToCleanupDirectoriesRequestDelegate : RespondToCleanupRequestDelegate<ProductDirectory>
    {
        [Dependencies(
            typeof(Itemize.ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate),
            typeof(Itemize.ItemizeAllUpdatedProductFilesAsyncDelegate),
            typeof(ItemizePassthroughDelegate),
            typeof(FormatValidationFileDelegate),
            typeof(DeleteToRecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToCleanupDirectoriesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductDirectoriesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductDirectoriesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IDeleteDelegate<string> deleteDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllExpectedProductDirectoriesAsyncDelegate,
                itemizeAllActualProductDirectoriesAsyncDelegate,
                itemizeDetailsDelegate,
                formatSupplementaryItemDelegate,
                deleteDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}