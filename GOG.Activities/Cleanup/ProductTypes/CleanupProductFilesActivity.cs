using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Directory;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace GOG.Activities.Cleanup.ProductTypes
{
    public class CleanupProductFilesActivity : CleanupActivity<ProductFile>
    {
		[Dependencies(
			"GOG.Delegates.Itemize.ItemizeAllGameDetailsDirectoriesAsyncDelegate,GOG.Delegates",
			"GOG.Delegates.Itemize.ItemizeAllProductFilesDirectoriesAsyncDelegate,GOG.Delegates",
			"Delegates.Itemize.ItemizeDirectoryFilesDelegate,Delegates",
			"Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
			"Delegates.Recycle.RecycleDelegate,Delegates",
			"Controllers.Directory.DirectoryController,Controllers",
			"Controllers.Status.StatusController,Controllers")]
        public CleanupProductFilesActivity(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductFilesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductFilesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IDirectoryController directoryController,
            IStatusController statusController) :
            base(
                itemizeAllExpectedProductFilesAsyncDelegate,
                itemizeAllActualProductFilesAsyncDelegate,
                itemizeDetailsDelegate,
                formatSupplementaryItemDelegate,
                recycleDelegate,
                directoryController,
                statusController)
        {
            // ...
        }
    }
}
