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

using Interfaces.Status;

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
        private readonly ISessionLogController sessionLogController;

        public RespondToUpdatePageResultRequestDelegate(
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IRecordsController<string> activityRecordsController,
            ISessionLogController sessionLogController)
        {
            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;
            this.activityRecordsController = activityRecordsController;
            this.sessionLogController = sessionLogController;
        }

        public async Task<ISessionLog> RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            sessionLogController.StartSession("Updating products");

            // TODO: Figure out better way to identify and activity
            await activityRecordsController.SetRecordAsync("PageResultUpdateActivity", RecordsTypes.Started);

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

            // var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extracting...");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            // await statusController.CompleteAsync(extractTask);

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

            return sessionLogController.CompleteSession();
        }
    }
}
