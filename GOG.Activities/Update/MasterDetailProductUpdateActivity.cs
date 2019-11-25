using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;
using Interfaces.Models.Entities;

using GOG.Interfaces.Delegates.GetUpdateIdentity;
using GOG.Interfaces.Delegates.FillGaps;
// using GOG.Interfaces.Delegates.GetUpdateUri;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.Update
{
    public class MasterDetailProductUpdateActivity<MasterType, DetailType> :
        Activity
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        readonly IDataController<MasterType> masterDataController;
        readonly IDataController<DetailType> detailDataController;
        IIndexController<long> updatedDataController;

        readonly IItemizeAllAsyncDelegate<MasterType> itemizeMasterTypeGapsAsyncDelegate;

        readonly IGetDeserializedAsyncDelegate<DetailType> getDeserializedDelegate;

        readonly IGetUpdateIdentityDelegate<MasterType> getUpdateIdentityDelegate;
        readonly IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        // Entity context;
        // readonly IGetUpdateUriDelegate<Entity> getUpdateUriDelegate;
        private readonly IGetValueDelegate<string> getUpdateUriDelegate;

        readonly string updateTypeDescription;

        public MasterDetailProductUpdateActivity(
            // Entity context,
            IGetValueDelegate<string> getUpdateUriDelegate,
            IItemizeAllAsyncDelegate<MasterType> itemizeMasterTypeGapsAsyncDelegate,
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

            this.itemizeMasterTypeGapsAsyncDelegate = itemizeMasterTypeGapsAsyncDelegate;

            this.getDeserializedDelegate = getDeserializedDelegate;

            this.getUpdateIdentityDelegate = getUpdateIdentityDelegate;
            this.fillGapsDelegate = fillGapsDelegate;

            // this.context = context;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            updateTypeDescription = typeof(DetailType).Name;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsTask = await statusController.CreateAsync(status, $"Update {updateTypeDescription}");

            // We'll limit detail updates to user specified ids.
            // if user didn't provide a list of ids - we'll use the details gaps 
            // (ids that exist in master list, but not detail) and updated

            var currentProduct = 0;

            await foreach (var product in itemizeMasterTypeGapsAsyncDelegate.ItemizeAllAsync(updateProductsTask))
            {
                if (product == null) continue;

                await statusController.UpdateProgressAsync(
                    updateProductsTask,
                    ++currentProduct,
                    0, // TODO: Probably need to figure a better way to report progress on unknown sets
                    product.Title);

                var updateIdentity = getUpdateIdentityDelegate.GetUpdateIdentity(product);
                if (string.IsNullOrEmpty(updateIdentity)) continue;

                var uri = string.Format(
                    getUpdateUriDelegate.GetValue(),
                    updateIdentity);

                var data = await getDeserializedDelegate.GetDeserializedAsync(updateProductsTask, uri);

                if (data != null)
                {
                    fillGapsDelegate?.FillGaps(data, product);
                    await detailDataController.UpdateAsync(data, updateProductsTask);
                }
            }

            await detailDataController.CommitAsync(updateProductsTask);

            await statusController.CompleteAsync(updateProductsTask);
        }
    }
}
