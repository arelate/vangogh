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
        private readonly IResponseLogController responseLogController;

        public RespondToUpdatePageResultRequestDelegate(
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

        public async Task<IResponseLog> RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            responseLogController.OpenResponseLog("Updating products");

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Started);

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

            // var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extracting...");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            // await statusController.CompleteAsync(extractTask);

            if (newProducts.Any())
            {
                responseLogController.StartAction("Saving new products");

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

            return responseLogController.CloseResponseLog();
        }
    }
}
