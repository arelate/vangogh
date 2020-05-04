using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using Delegates.Data.Storage;
using Delegates.Activities;
using Delegates.Conversions.Uris;
using Delegates.Itemizations;
using GOG.Delegates.Itemizations;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Server.Cleanup
{
    [RespondsToRequests(Method = "cleanup", Collection = "directories")]
    public class CleanupDirectoriesAsyncDelegate : CleanupAsyncDelegate<ProductDirectory>
    {
        [Dependencies(
            typeof(ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate),
            typeof(ItemizeAllUpdatedProductFilesAsyncDelegate),
            typeof(ItemizePassthroughDelegate),
            typeof(ConvertFilePathToValidationFilePathDelegate),
            typeof(DeleteToRecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public CleanupDirectoriesAsyncDelegate(
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