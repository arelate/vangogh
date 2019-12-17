using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Activities.Update
{
    public abstract class UpdatePageResultActivity<PageType, DataType> : Activity
        where PageType : Models.PageResult
        where DataType : ProductCore
    {

        readonly IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        readonly IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        readonly IDataController<DataType> dataController;
        readonly IRecordsController<string> activityRecordsController;

        public UpdatePageResultActivity(
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IRecordsController<string> activityRecordsController,
            IStatusController statusController) :
            base(statusController)
        {
            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;
            this.activityRecordsController = activityRecordsController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllProductsTask = await statusController.CreateAsync(status, $"Updating...");

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Started, updateAllProductsTask);

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync(updateAllProductsTask);

            var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extracting...");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            await statusController.CompleteAsync(extractTask);

            if (newProducts.Any())
            {
                var updateTask = await statusController.CreateAsync(updateAllProductsTask, $"Saving...");
                var current = 0;
                var updateProgressEvery = 10;

                foreach (var product in newProducts)
                {
                    if (++current % updateProgressEvery == 0)
                        await statusController.UpdateProgressAsync(updateTask, current, newProducts.Count(), product.Title);

                    await dataController.UpdateAsync(product, updateTask);
                }

                await statusController.CompleteAsync(updateTask);
            }

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Completed, updateAllProductsTask);

            await dataController.CommitAsync(updateAllProductsTask);

            await statusController.CompleteAsync(updateAllProductsTask);
        }
    }
}
