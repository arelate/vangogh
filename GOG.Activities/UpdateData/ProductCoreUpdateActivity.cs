using System.Collections.Generic;
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
    public class ProductCoreUpdateActivity<UpdateType, ListType> :
        Activity
        where ListType : ProductCore
        where UpdateType : ProductCore
    {
        private IDataController<UpdateType> updateTypeDataController;
        private IDataController<ListType> listTypeDataController;
        private IDataController<long> updatedDataController;

        private IGetDeserializedAsyncDelegate<UpdateType> getDeserializedDelegate;

        private IGetUpdateIdentityDelegate<ListType> getUpdateIdentityDelegate;
        private IConnectDelegate<UpdateType, ListType> connectDelegate;

        private Context context;
        private IGetUpdateUriDelegate<Context> getUpdateUriDelegate;

        private string updateTypeDescription;

        public ProductCoreUpdateActivity(
            Context context,
            IGetUpdateUriDelegate<Context> getUpdateUriDelegate,
            IDataController<UpdateType> updateTypeDataController,
            IDataController<ListType> listTypeDataController,
            IDataController<long> updatedDataController,
            IGetDeserializedAsyncDelegate<UpdateType> getDeserializedDelegate,
            IGetUpdateIdentityDelegate<ListType> getUpdateIdentityDelegate,
            IStatusController statusController,
            IConnectDelegate<UpdateType, ListType> connectDelegate = null) :
            base(statusController)
        {
            this.updateTypeDataController = updateTypeDataController;
            this.listTypeDataController = listTypeDataController;
            this.updatedDataController = updatedDataController;

            this.getDeserializedDelegate = getDeserializedDelegate;

            this.getUpdateIdentityDelegate = getUpdateIdentityDelegate;
            this.connectDelegate = connectDelegate;

            this.context = context;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            updateTypeDescription = typeof(UpdateType).Name;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsTask = statusController.Create(status, $"Update {updateTypeDescription}");

            var updatedProducts = new List<long>();

            var missingDataEnumerationTask = statusController.Create(updateProductsTask, "Enumerate gaps");

            foreach (var id in listTypeDataController.EnumerateIds())
            {
                if (!updateTypeDataController.ContainsId(id))
                    updatedProducts.Add(id);
            }

            statusController.Complete(missingDataEnumerationTask);

            var addUpdatedTask = statusController.Create(updateProductsTask, "Add new/updated");

            updatedProducts.AddRange(updatedDataController.EnumerateIds());

            statusController.Complete(addUpdatedTask);

            var currentProduct = 0;

            foreach (var id in updatedProducts)
            {
                var product = await listTypeDataController.GetByIdAsync(id);
                if (product == null) continue;

                statusController.UpdateProgress(
                    updateProductsTask,
                    ++currentProduct,
                    updatedProducts.Count,
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
                    await updateTypeDataController.UpdateAsync(updateProductsTask, data);
                }
            }

            statusController.Complete(updateProductsTask);
        }
    }
}
