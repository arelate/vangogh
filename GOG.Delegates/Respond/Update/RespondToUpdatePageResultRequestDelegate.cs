using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Delegates.Respond.Update
{
    public abstract class RespondToUpdatePageResultRequestDelegate<PageType, DataType> : IRespondAsyncDelegate
        where PageType : Models.PageResult
        where DataType : ProductCore
    {
        private readonly IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        private readonly IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        // private readonly IDataController<DataType> dataController;
        private readonly IUpdateAsyncDelegate<DataType> updateDataAsyncDelegate;
        private readonly ICommitAsyncDelegate commitDataAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public RespondToUpdatePageResultRequestDelegate(
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
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

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Update products");

            var activityDescription = $"Update {typeof(DataType)}";

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync();

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