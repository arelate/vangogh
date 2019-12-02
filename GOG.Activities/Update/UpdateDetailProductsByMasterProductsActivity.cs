using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using GOG.Interfaces.Delegates.FillGaps;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.Update
{
    public abstract class UpdateDetailProductsByMasterProductsActivity<DetailType, MasterType> :
        Activity
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        readonly IDataController<DetailType> detailDataController;
        readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate;

        readonly IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate;

        readonly IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate;
        readonly IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        private readonly IGetValueDelegate<string> getDetailUpdateUriDelegate;

        readonly string updateTypeDescription;

        public UpdateDetailProductsByMasterProductsActivity(
            IGetValueDelegate<string> getDetailUpdateUriDelegate,
            IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate,
            IDataController<DetailType> detailDataController,
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate,
            IStatusController statusController,
            IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate = null) :
            base(statusController)
        {
            this.detailDataController = detailDataController;
            this.itemizeAllMasterDetailGapsAsyncDelegate = itemizeAllMasterDetailGapsAsyncDelegate;

            this.getDeserializedDetailAsyncDelegate = getDeserializedDetailAsyncDelegate;

            this.convertMasterTypeToDetailUpdateIdentityDelegate = convertMasterTypeToDetailUpdateIdentityDelegate;
            this.fillGapsDelegate = fillGapsDelegate;

            this.getDetailUpdateUriDelegate = getDetailUpdateUriDelegate;
            updateTypeDescription = typeof(DetailType).Name;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsTask = await statusController.CreateAsync(status, $"Update {updateTypeDescription}");

            // We'll limit detail updates to user specified ids.
            // if user didn't provide a list of ids - we'll use the details gaps 
            // (ids that exist in master list, but not detail) and updated

            var currentProduct = 0;

            await foreach (var masterProductWithoutDetail in itemizeAllMasterDetailGapsAsyncDelegate.ItemizeAllAsync(updateProductsTask))
            {
                await statusController.UpdateProgressAsync(
                    updateProductsTask,
                    ++currentProduct,
                    0, // TODO: Probably need to figure a better way to report progress on unknown sets
                    masterProductWithoutDetail.Title);

                var detailUpdateIdentity = convertMasterTypeToDetailUpdateIdentityDelegate.Convert(masterProductWithoutDetail);
                if (string.IsNullOrEmpty(detailUpdateIdentity)) continue;

                var detailUpdateUri = string.Format(
                    getDetailUpdateUriDelegate.GetValue(),
                    detailUpdateIdentity);

                var detailData = await getDeserializedDetailAsyncDelegate.GetDeserializedAsync(updateProductsTask, detailUpdateUri);

                if (detailData != null)
                {
                    if (fillGapsDelegate != null)
                        fillGapsDelegate.FillGaps(detailData, masterProductWithoutDetail);

                    await detailDataController.UpdateAsync(detailData, updateProductsTask);
                }
            }

            await detailDataController.CommitAsync(updateProductsTask);

            await statusController.CompleteAsync(updateProductsTask);
        }
    }
}
