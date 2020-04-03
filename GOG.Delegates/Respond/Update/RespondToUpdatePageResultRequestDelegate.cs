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

namespace GOG.Delegates.Respond.Update
{
    public abstract class RespondToUpdatePageResultRequestDelegate<PageType, DataType> : IRespondAsyncDelegate
        where PageType : Models.PageResult
        where DataType : ProductCore
    {
        readonly IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        readonly IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        readonly IDataController<DataType> dataController;
        private readonly IActionLogController actionLogController;

        public RespondToUpdatePageResultRequestDelegate(
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IActionLogController actionLogController)
        {
            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            actionLogController.StartAction("Update products");

            var activityDescription = $"Update {typeof(DataType)}";

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);

            if (newProducts.Any())
            {
                actionLogController.StartAction("Save new products");

                foreach (var product in newProducts)
                {
                    actionLogController.IncrementActionProgress();
                    await dataController.UpdateAsync(product);
                }

                actionLogController.CompleteAction();
            }

            await dataController.CommitAsync();

            actionLogController.CompleteAction();
        }
    }
}
