using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using Delegates.Data.Storage;
using Delegates.Activities;
using Delegates.Conversions.Uris;
using Delegates.Itemizations;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Respond.Cleanup.ProductTypes
{
    [RespondsToRequests(Method = "cleanup", Collection = "directories")]
    public class RespondToCleanupDirectoriesRequestDelegate : RespondToCleanupRequestDelegate<ProductDirectory>
    {
        [Dependencies(
            typeof(Itemize.ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate),
            typeof(Itemize.ItemizeAllUpdatedProductFilesAsyncDelegate),
            typeof(ItemizePassthroughDelegate),
            typeof(ConvertFilePathToValidationFilePathDelegate),
            typeof(DeleteToRecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToCleanupDirectoriesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductDirectoriesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductDirectoriesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IConvertDelegate<string, string> convertFilePathToValidationFilePathDelegate,
            IDeleteDelegate<string> deleteDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllExpectedProductDirectoriesAsyncDelegate,
                itemizeAllActualProductDirectoriesAsyncDelegate,
                itemizeDetailsDelegate,
                convertFilePathToValidationFilePathDelegate,
                deleteDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}