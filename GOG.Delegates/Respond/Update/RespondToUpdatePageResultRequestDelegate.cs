using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Logs;

using Interfaces.Models.RecordsTypes;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    public abstract class RespondToUpdatePageResultRequestDelegate<PageType, DataType> : IRespondAsyncDelegate
        where PageType : Models.PageResult
        where DataType : ProductCore
    {
        readonly IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        readonly IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        readonly IDataController<DataType> dataController;
        readonly IRecordsController<string> activityRecordsController;
        private readonly IActionLogController actionLogController;

        public RespondToUpdatePageResultRequestDelegate(
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

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            actionLogController.StartAction("Updating products");

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Started);

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

            // var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extracting...");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            // await statusController.CompleteAsync(extractTask);

            if (newProducts.Any())
            {
                actionLogController.StartAction("Saving new products");

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
