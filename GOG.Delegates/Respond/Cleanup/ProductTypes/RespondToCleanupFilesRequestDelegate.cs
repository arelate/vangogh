using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using Delegates.Format.Uri;
using Delegates.Data.Storage;
using Delegates.Activities;
using Delegates.Itemize;
using Interfaces.Delegates.Data;

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
            typeof(DeleteToRecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToCleanupFilesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductFilesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductFilesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IDeleteDelegate<string> deleteDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllExpectedProductFilesAsyncDelegate,
                itemizeAllActualProductFilesAsyncDelegate,
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