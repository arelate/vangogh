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
    [RespondsToRequests(Method = "cleanup", Collection = "files")]
    public class CleanupFilesAsyncDelegate : CleanupAsyncDelegate<ProductFile>
    {
        [Dependencies(
            typeof(ItemizeAllGameDetailsDirectoriesAsyncDelegate),
            typeof(ItemizeAllProductFilesDirectoriesAsyncDelegate),
            typeof(ItemizeDirectoryFilesDelegate),
            typeof(ConvertFilePathToValidationFilePathDelegate),
            typeof(DeleteToRecycleDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public CleanupFilesAsyncDelegate(
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