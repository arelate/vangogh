using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Respond;
using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.FillGaps;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.Respond.Update
{
    public abstract class RespondToUpdateMasterDetailsRequestDelegate<DetailType, MasterType> : IRespondAsyncDelegate
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        private readonly IDataController<DetailType> detailDataController;
        private readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate;

        private readonly IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate;

        private readonly IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate;
        private readonly IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        private readonly IGetValueDelegate<string> getDetailUpdateUriDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public RespondToUpdateMasterDetailsRequestDelegate(
            IGetValueDelegate<string> getDetailUpdateUriDelegate,
            IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate,
            IDataController<DetailType> detailDataController,
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate,
            IGetDeserializedAsyncDelegate<DetailType> getDeserializedDetailAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate,
            IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate = null)
        {
            this.detailDataController = detailDataController;
            this.itemizeAllMasterDetailGapsAsyncDelegate = itemizeAllMasterDetailGapsAsyncDelegate;

            this.getDeserializedDetailAsyncDelegate = getDeserializedDetailAsyncDelegate;

            this.convertMasterTypeToDetailUpdateIdentityDelegate = convertMasterTypeToDetailUpdateIdentityDelegate;
            this.fillGapsDelegate = fillGapsDelegate;

            this.getDetailUpdateUriDelegate = getDetailUpdateUriDelegate;

            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start($"Update {typeof(DetailType).Name}");

            await foreach (var masterProductWithoutDetail in
                itemizeAllMasterDetailGapsAsyncDelegate.ItemizeAllAsync())
            {
                setProgressDelegate.SetProgress();

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

            completeDelegate.Complete();
        }
    }
}