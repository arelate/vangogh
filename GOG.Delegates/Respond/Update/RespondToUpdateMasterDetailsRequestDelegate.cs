using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.FillGaps;
using Models.ProductTypes;

namespace GOG.Delegates.Respond.Update
{
    public abstract class RespondToUpdateMasterDetailsRequestDelegate<DetailType, MasterType> : IRespondAsyncDelegate
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        private readonly IUpdateAsyncDelegate<DetailType> updateDetailDataAsyncDelegate;
        private readonly ICommitAsyncDelegate commitDetailDataAsyncDelegate;
        private readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate;

        private readonly IGetDataAsyncDelegate<DetailType, string> getDeserializedDetailAsyncDelegate;

        private readonly IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate;
        private readonly IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate;

        private readonly IGetValueDelegate<string, string> getDetailUpdateUriDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public RespondToUpdateMasterDetailsRequestDelegate(
            IGetValueDelegate<string, string> getDetailUpdateUriDelegate,
            IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate,
            IUpdateAsyncDelegate<DetailType> updateDetailAsyncDelegate,
            ICommitAsyncDelegate commitAsyncDelegate,
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate,
            IGetDataAsyncDelegate<DetailType, string> getDeserializedDetailAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate,
            IFillGapsDelegate<DetailType, MasterType> fillGapsDelegate = null)
        {
            updateDetailDataAsyncDelegate = updateDetailAsyncDelegate;
            commitDetailDataAsyncDelegate = commitAsyncDelegate;
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
                    getDetailUpdateUriDelegate.GetValue(string.Empty),
                    detailUpdateIdentity);

                var detailData = await getDeserializedDetailAsyncDelegate.GetDataAsync(detailUpdateUri);

                if (detailData != null)
                {
                    if (fillGapsDelegate != null)
                        fillGapsDelegate.FillGaps(detailData, masterProductWithoutDetail);

                    await updateDetailDataAsyncDelegate.UpdateAsync(detailData);
                }
            }

            await commitDetailDataAsyncDelegate.CommitAsync();

            completeDelegate.Complete();
        }
    }
}