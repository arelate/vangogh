using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Interfaces.Activity;

using GOG.Interfaces.Delegates.FillGaps;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.Update
{
    public abstract class UpdateDetailProductsByMasterProductsActivity<DetailType, MasterType> : IActivity
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        readonly IDataController<DetailType> detailDataController;
        readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate;

        readonly IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate;

        readonly IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate;
        readonly IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        private readonly IGetValueDelegate<string> getDetailUpdateUriDelegate;
        private readonly IResponseLogController responseLogController;


        public UpdateDetailProductsByMasterProductsActivity(
            IGetValueDelegate<string> getDetailUpdateUriDelegate,
            IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate,
            IDataController<DetailType> detailDataController,
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate,
            IResponseLogController responseLogController,
            IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate = null)
        {
            this.detailDataController = detailDataController;
            this.itemizeAllMasterDetailGapsAsyncDelegate = itemizeAllMasterDetailGapsAsyncDelegate;

            this.getDeserializedDetailAsyncDelegate = getDeserializedDetailAsyncDelegate;

            this.convertMasterTypeToDetailUpdateIdentityDelegate = convertMasterTypeToDetailUpdateIdentityDelegate;
            this.fillGapsDelegate = fillGapsDelegate;

            this.getDetailUpdateUriDelegate = getDetailUpdateUriDelegate;
            this.responseLogController = responseLogController;
        }

        public async Task ProcessActivityAsync()
        {
            responseLogController.OpenResponseLog($"Update {typeof(DetailType).Name}");

            // We'll limit detail updates to user specified ids.
            // if user didn't provide a list of ids - we'll use the details gaps 
            // (ids that exist in master list, but not detail) and updated

            await foreach (var masterProductWithoutDetail in
                itemizeAllMasterDetailGapsAsyncDelegate.ItemizeAllAsync())
            {
                responseLogController.IncrementActionProgress();

                var detailUpdateIdentity =
                    convertMasterTypeToDetailUpdateIdentityDelegate.Convert(
                        masterProductWithoutDetail);

                if (string.IsNullOrEmpty(detailUpdateIdentity)) continue;

                var detailUpdateUri = string.Format(
                    getDetailUpdateUriDelegate.GetValue(),
                    detailUpdateIdentity);

                var detailData = await getDeserializedDetailAsyncDelegate.GetDeserializedAsync(detailUpdateUri);

                if (detailData != null)
                {
                    if (fillGapsDelegate != null)
                        fillGapsDelegate.FillGaps(detailData, masterProductWithoutDetail);

                    await detailDataController.UpdateAsync(detailData);
                }
            }

            await detailDataController.CommitAsync();

            responseLogController.CloseResponseLog();
        }
    }
}