using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Models.RecordsTypes;

using Interfaces.Activity;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Activities.Update
{
    public abstract class UpdatePageResultActivity<PageType, DataType> : IActivity
        where PageType : Models.PageResult
        where DataType : ProductCore
    {
        readonly IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        readonly IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        readonly IDataController<DataType> dataController;
        readonly IRecordsController<string> activityRecordsController;
        readonly IActionLogController actionLogController;

        public UpdatePageResultActivity(
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IRecordsController<string> activityRecordsController,
            IActionLogController actionLogController)
        {
            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;
            this.activityRecordsController = activityRecordsController;
            this.actionLogController = actionLogController;
        }

        public async Task ProcessActivityAsync()
        {
            actionLogController.StartAction($"Updating...");

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Started);

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

            actionLogController.StartAction($"Extracting...");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            actionLogController.CompleteAction();

            if (newProducts.Any())
            {
                actionLogController.StartAction($"Saving...");

                foreach (var product in newProducts)
                {
                    actionLogController.IncrementActionProgress();
                    await dataController.UpdateAsync(product);
                }

                actionLogController.CompleteAction();
            }

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Completed);

            await dataController.CommitAsync();

            actionLogController.CompleteAction();
        }
    }
}
