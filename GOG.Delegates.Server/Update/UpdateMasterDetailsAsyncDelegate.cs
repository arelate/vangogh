using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Server;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace GOG.Delegates.Server.Update
{
    public abstract class UpdateMasterDetailsAsyncDelegate<DetailType, MasterType> : IProcessAsyncDelegate
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        private readonly IUpdateAsyncDelegate<DetailType> updateDetailDataAsyncDelegate;
        private readonly ICommitAsyncDelegate commitDetailDataAsyncDelegate;
        private readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate;

        private readonly IGetDataAsyncDelegate<DetailType, string> getDeserializedDetailAsyncDelegate;

        private readonly IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate;

        private readonly IGetValueDelegate<string, string> getDetailUpdateUriDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public UpdateMasterDetailsAsyncDelegate(
            IGetValueDelegate<string, string> getDetailUpdateUriDelegate,
            IConvertDelegate<MasterType, string> convertMasterTypeToDetailUpdateIdentityDelegate,
            IUpdateAsyncDelegate<DetailType> updateDetailAsyncDelegate,
            ICommitAsyncDelegate commitAsyncDelegate,
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDetailGapsAsyncDelegate,
            IGetDataAsyncDelegate<DetailType, string> getDeserializedDetailAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            updateDetailDataAsyncDelegate = updateDetailAsyncDelegate;
            commitDetailDataAsyncDelegate = commitAsyncDelegate;
            this.itemizeAllMasterDetailGapsAsyncDelegate = itemizeAllMasterDetailGapsAsyncDelegate;

            this.getDeserializedDetailAsyncDelegate = getDeserializedDetailAsyncDelegate;

            this.convertMasterTypeToDetailUpdateIdentityDelegate = convertMasterTypeToDetailUpdateIdentityDelegate;

            this.getDetailUpdateUriDelegate = getDetailUpdateUriDelegate;

            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
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
                    // GOG.com quirk
                    // GameDetails are requested using xxxxxxxxx.json Uri, 
                    // where xxxxxxxxx is Id that comes from AccountProduct.
                    // Actual GameDetails payload doesn't contain Id,
                    // so this delegate "associates" GameDetails to AccountProduct
                    if (detailData.Id == 0)
                        detailData.Id = masterProductWithoutDetail.Id;

                    await updateDetailDataAsyncDelegate.UpdateAsync(detailData);
                }
            }

            await commitDetailDataAsyncDelegate.CommitAsync();

            completeDelegate.Complete();
        }
    }
}