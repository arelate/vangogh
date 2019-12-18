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
        readonly IResponseLogController responseLogController;

        public UpdatePageResultActivity(
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IRecordsController<string> activityRecordsController,
            IResponseLogController responseLogController)
        {
            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;
            this.activityRecordsController = activityRecordsController;
            this.responseLogController = responseLogController;
        }

        public async Task ProcessActivityAsync()
        {
            responseLogController.OpenResponseLog($"Updating...");

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Started);

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

            responseLogController.StartAction($"Extracting...");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            responseLogController.CompleteAction();

            if (newProducts.Any())
            {
                responseLogController.StartAction($"Saving...");

                foreach (var product in newProducts)
                {
                    responseLogController.IncrementActionProgress();
                    await dataController.UpdateAsync(product);
                }

                responseLogController.CompleteAction();
            }

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Completed);

            await dataController.CommitAsync();

            responseLogController.CloseResponseLog();
        }
    }
}
