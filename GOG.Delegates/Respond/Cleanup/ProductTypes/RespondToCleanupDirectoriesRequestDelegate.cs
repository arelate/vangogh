using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace GOG.Delegates.Respond.Cleanup.ProductTypes
{
    [RespondsToRequests(Method = "cleanup", Collection = "directories")]
    public class RespondToCleanupDirectoriesRequestDelegate : RespondToCleanupRequestDelegate<ProductDirectory>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Delegates.Itemize.ItemizeAllUpdatedGameDetailsManualUrlFilesAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeAllUpdatedProductFilesAsyncDelegate,GOG.Delegates",
            "Delegates.Itemize.ItemizePassthroughDelegate,Delegates",
            "Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
            "Delegates.Recycle.RecycleDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToCleanupDirectoriesRequestDelegate(
            IItemizeAllAsyncDelegate<string> itemizeAllExpectedProductDirectoriesAsyncDelegate,
            IItemizeAllAsyncDelegate<string> itemizeAllActualProductDirectoriesAsyncDelegate,
            IItemizeDelegate<string, string> itemizeDetailsDelegate,
            IFormatDelegate<string, string> formatSupplementaryItemDelegate,
            IRecycleDelegate recycleDelegate,
            IActionLogController actionLogController) :
            base(
                itemizeAllExpectedProductDirectoriesAsyncDelegate,
                itemizeAllActualProductDirectoriesAsyncDelegate,
                itemizeDetailsDelegate,
                formatSupplementaryItemDelegate,
                recycleDelegate,
                actionLogController)
        {
            // ...
        }
    }
}
