using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace GOG.Activities.Cleanup.ProductTypes
{
    public class CleanupProductDirectoriesActivity : CleanupActivity<ProductDirectory>
    {
        [Dependencies(
			"GOG.Delegates.Itemize.ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate,GOG.Delegates",
			"GOG.Delegates.Itemize.ItemizeAllUpdatedProductFilesAsyncDelegate,GOG.Delegates",
			"Delegates.Itemize.ItemizePassthroughDelegate,Delegates",
			"Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
			"Delegates.Recycle.RecycleDelegate,Delegates",
			"Controllers.Directory.DirectoryController,Controllers",
			"Controllers.Logs.ResponseLogController,Controllers")]
        public CleanupProductDirectoriesActivity(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductDirectoriesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductDirectoriesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IDirectoryController directoryController,
            IResponseLogController responseLogController) :
            base(
                itemizeAllExpectedProductDirectoriesAsyncDelegate,
                itemizeAllActualProductDirectoriesAsyncDelegate,
                itemizeDetailsDelegate,
                formatSupplementaryItemDelegate,
                recycleDelegate,
                directoryController,
                responseLogController)
        {
            // ...
        }
    }
}
