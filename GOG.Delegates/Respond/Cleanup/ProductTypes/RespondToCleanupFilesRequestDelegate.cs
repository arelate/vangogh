using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using Delegates.Format.Uri;
using Delegates.Recycle;
using Delegates.Activities;
using Delegates.Itemize;

namespace GOG.Delegates.Respond.Cleanup.ProductTypes
{
    [RespondsToRequests(Method = "cleanup", Collection = "files")]
    public class RespondToCleanupFilesRequestDelegate : RespondToCleanupRequestDelegate<ProductFile>
    {
        [Dependencies(
            typeof(Itemize.ItemizeAllGameDetailsDirectoriesAsyncDelegate),
            typeof(Itemize.ItemizeAllProductFilesDirectoriesAsyncDelegate),
            typeof(ItemizeDirectoryFilesDelegate),
            typeof(FormatValidationFileDelegate),
            typeof(RecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToCleanupFilesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductFilesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductFilesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllExpectedProductFilesAsyncDelegate,
                itemizeAllActualProductFilesAsyncDelegate,
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