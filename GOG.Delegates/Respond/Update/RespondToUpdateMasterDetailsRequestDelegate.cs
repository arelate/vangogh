using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using GOG.Interfaces.Delegates.FillGaps;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.Respond.Update
{
    public abstract class RespondToUpdateMasterDetailsRequestDelegate<DetailType, MasterType> : IRespondAsyncDelegate
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        readonly IDataController<DetailType> detailDataController;
        readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate;

        readonly IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate;

        readonly IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate;
        readonly IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        private readonly IGetValueDelegate<string> getDetailUpdateUriDelegate;
        private readonly IActionLogController actionLogController;

        public RespondToUpdateMasterDetailsRequestDelegate(
            IGetValueDelegate<string> getDetailUpdateUriDelegate,
            IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate,
            IDataController<DetailType> detailDataController,
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate,
            IActionLogController actionLogController,
            IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate = null)
        {
            this.detailDataController = detailDataController;
            this.itemizeAllMasterDetailGapsAsyncDelegate = itemizeAllMasterDetailGapsAsyncDelegate;

            this.getDeserializedDetailAsyncDelegate = getDeserializedDetailAsyncDelegate;

            this.convertMasterTypeToDetailUpdateIdentityDelegate = convertMasterTypeToDetailUpdateIdentityDelegate;
            this.fillGapsDelegate = fillGapsDelegate;

            this.getDetailUpdateUriDelegate = getDetailUpdateUriDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            actionLogController.StartAction($"Update {typeof(DetailType).Name}");

            await foreach (var masterProductWithoutDetail in
                itemizeAllMasterDetailGapsAsyncDelegate.ItemizeAllAsync())
            {
                actionLogController.IncrementActionProgress();

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

            actionLogController.CompleteAction();
        }
    }
}