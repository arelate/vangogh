using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Server;
using Models.ProductTypes;

namespace GOG.Delegates.Server.Update
{
    public abstract class UpdatePageResultAsyncDelegate<PageType, DataType> : IProcessAsyncDelegate
        where PageType : Models.PageResult
        where DataType : ProductCore
    {
        private readonly IGetDataAsyncDelegate<IList<PageType>, string> getPageResultsAsyncDelegate;
        private readonly IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        // private readonly IDataController<DataType> dataController;
        private readonly IUpdateAsyncDelegate<DataType> updateDataAsyncDelegate;
        private readonly ICommitAsyncDelegate commitDataAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public UpdatePageResultAsyncDelegate(
            IGetDataAsyncDelegate<IList<PageType>, string> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IUpdateAsyncDelegate<DataType> updateDataAsyncDelegate,
            ICommitAsyncDelegate commitDataAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.updateDataAsyncDelegate = updateDataAsyncDelegate;
            this.commitDataAsyncDelegate = commitDataAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Update products");

            var activityDescription = $"Update {typeof(DataType)}";

            var productsPageResults = 
                await getPageResultsAsyncDelegate.GetDataAsync(string.Empty);

            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);

            if (newProducts.Any())
            {
                startDelegate.Start("Save new products");

                foreach (var product in newProducts)
                {
                    setProgressDelegate.SetProgress();
                    await updateDataAsyncDelegate.UpdateAsync(product);
                }

                completeDelegate.Complete();
            }

            await commitDataAsyncDelegate.CommitAsync();

            completeDelegate.Complete();
        }
    }
}