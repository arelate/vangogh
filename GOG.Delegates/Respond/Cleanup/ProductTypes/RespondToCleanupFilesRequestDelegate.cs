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
    [RespondsToRequests(Method = "cleanup", Collection = "files")]
    public class RespondToCleanupFilesRequestDelegate : RespondToCleanupRequestDelegate<ProductFile>
    {
        [Dependencies(
            typeof(Itemize.ItemizeAllGameDetailsDirectoriesAsyncDelegate),
            typeof(Itemize.ItemizeAllProductFilesDirectoriesAsyncDelegate),
            typeof(ItemizeDirectoryFilesDelegate),
            typeof(ConvertFilePathToValidationFilePathDelegate),
            typeof(DeleteToRecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToCleanupFilesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductFilesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductFilesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IConvertDelegate<string, string> convertFilePathToValidationFilePathDelegate,
            IDeleteDelegate<string> deleteDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllExpectedProductFilesAsyncDelegate,
                itemizeAllActualProductFilesAsyncDelegate,
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