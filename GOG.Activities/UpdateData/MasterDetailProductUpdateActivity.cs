using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;
using Interfaces.ContextDefinitions;

using GOG.Interfaces.Delegates.GetUpdateIdentity;
using GOG.Interfaces.Delegates.FillGaps;
using GOG.Interfaces.Delegates.GetUpdateUri;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.UpdateData
{
    public class MasterDetailProductUpdateActivity<MasterType, DetailType> :
        Activity
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        private IDataController<MasterType> masterDataController;
        private IDataController<DetailType> detailDataController;
        private IIndexController<long> updatedDataController;

        private IItemizeAllAsyncDelegate<long> itemizeUserRequestedOrDefaultAsyncDelegate;

        private IGetDeserializedAsyncDelegate<DetailType> getDeserializedDelegate;

        private IGetUpdateIdentityDelegate<MasterType> getUpdateIdentityDelegate;
        private IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        private Context context;
        private IGetUpdateUriDelegate<Context> getUpdateUriDelegate;

        private string updateTypeDescription;

        public MasterDetailProductUpdateActivity(
            Context context,
            IGetUpdateUriDelegate<Context> getUpdateUriDelegate,
            IItemizeAllAsyncDelegate<long> itemizeUserRequestedOrDefaultAsyncDelegate,
            IDataController<MasterType> masterDataController,
            IDataController<DetailType> detailDataController,
            IIndexController<long> updatedDataController,
            IGetDeserializedAsyncDelegate<DetailType> getDeserializedDelegate,
            IGetUpdateIdentityDelegate<MasterType> getUpdateIdentityDelegate,
            IStatusController statusController,
            IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate = null) :
            base(statusController)
        {
            this.masterDataController = masterDataController;
            this.detailDataController = detailDataController;
            this.updatedDataController = updatedDataController;

            this.itemizeUserRequestedOrDefaultAsyncDelegate = itemizeUserRequestedOrDefaultAsyncDelegate;

            this.getDeserializedDelegate = getDeserializedDelegate;

            this.getUpdateIdentityDelegate = getUpdateIdentityDelegate;
            this.fillGapsDelegate = fillGapsDelegate;

            this.context = context;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            updateTypeDescription = typeof(DetailType).Name;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsTask = await statusController.CreateAsync(status, $"Update {updateTypeDescription}");

            // We'll limit detail updates to user specified ids.
            // if user didn't provide a list of ids - we'll use the details gaps 
            // (ids that exist in master list, but not detail) and updated
            var productsUpdateList = await itemizeUserRequestedOrDefaultAsyncDelegate.ItemizeAllAsync(updateProductsTask);

            var currentProduct = 0;

            foreach (var id in productsUpdateList)
            {
                var product = await masterDataController.GetByIdAsync(id, updateProductsTask);
                if (product == null) continue;

                await statusController.UpdateProgressAsync(
                    updateProductsTask,
                    ++currentProduct,
                    productsUpdateList.Count(),
                    product.Title);

                var updateIdentity = getUpdateIdentityDelegate.GetUpdateIdentity(product);
                if (string.IsNullOrEmpty(updateIdentity)) continue;

                var uri = string.Format(
                    getUpdateUriDelegate.GetUpdateUri(context),
                    updateIdentity);

                var data = await getDeserializedDelegate.GetDeserializedAsync(updateProductsTask, uri);

                if (data != null)
                {
                    fillGapsDelegate?.FillGaps(data, product);
                    await detailDataController.UpdateAsync(updateProductsTask, data);
                }
            }

            await statusController.CompleteAsync(updateProductsTask);
        }
    }
}
