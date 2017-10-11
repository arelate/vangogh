using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Connection;
using Interfaces.UpdateUri;
using Interfaces.UpdateIdentity;
using Interfaces.Data;
using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.ProductCore;

namespace GOG.Activities.UpdateData
{
    public class MasterDetailProductUpdateActivity<MasterType, DetailType> :
        Activity
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        private IDataController<MasterType> masterDataController;
        private IDataController<DetailType> detailDataController;
        private IDataController<long> updatedDataController;

        private IEnumerateIdsDelegate userRequestedOrOtherEnumerateDelegate;

        private IGetDeserializedAsyncDelegate<DetailType> getDeserializedDelegate;

        private IGetUpdateIdentityDelegate<MasterType> getUpdateIdentityDelegate;
        private IConnectDelegate<DetailType, MasterType> connectDelegate;

        private Context context;
        private IGetUpdateUriDelegate<Context> getUpdateUriDelegate;

        private string updateTypeDescription;

        public MasterDetailProductUpdateActivity(
            Context context,
            IGetUpdateUriDelegate<Context> getUpdateUriDelegate,
            IEnumerateIdsDelegate userRequestedOrOtherEnumerateDelegate,
            IDataController<MasterType> masterDataController,
            IDataController<DetailType> detailDataController,
            IDataController<long> updatedDataController,
            IGetDeserializedAsyncDelegate<DetailType> getDeserializedDelegate,
            IGetUpdateIdentityDelegate<MasterType> getUpdateIdentityDelegate,
            IStatusController statusController,
            IConnectDelegate<DetailType, MasterType> connectDelegate = null) :
            base(statusController)
        {
            this.masterDataController = masterDataController;
            this.detailDataController = detailDataController;
            this.updatedDataController = updatedDataController;

            this.userRequestedOrOtherEnumerateDelegate = userRequestedOrOtherEnumerateDelegate;

            this.getDeserializedDelegate = getDeserializedDelegate;

            this.getUpdateIdentityDelegate = getUpdateIdentityDelegate;
            this.connectDelegate = connectDelegate;

            this.context = context;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            updateTypeDescription = typeof(DetailType).Name;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsTask = statusController.Create(status, $"Update {updateTypeDescription}");

            // We'll limit detail updates to user specified ids.
            // if user didn't provide a list of ids - we'll use the details gaps 
            // (ids that exist in master list, but not detail) and updated
            var productsUpdateList = userRequestedOrOtherEnumerateDelegate.EnumerateIds();

            var currentProduct = 0;

            foreach (var id in productsUpdateList)
            {
                var product = await masterDataController.GetByIdAsync(id);
                if (product == null) continue;

                statusController.UpdateProgress(
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
                    connectDelegate?.Connect(data, product);
                    await detailDataController.UpdateAsync(updateProductsTask, data);
                }
            }

            statusController.Complete(updateProductsTask);
        }
    }
}
