using System.Collections.Generic;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;
using Models.ProductTypes;

namespace Delegates.Itemizations.MasterDetail
{
    public abstract class ItemizeAllMasterDetailsGapsAsyncDelegate<MasterType> :
        IItemizeAllAsyncDelegate<MasterType>
        where MasterType : ProductCore
    {
        private readonly IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDataAsyncDelegate;
        private readonly IConfirmAsyncDelegate<long> confirmDetailDataContainsIdAsyncDelegate;

        public ItemizeAllMasterDetailsGapsAsyncDelegate(
            IItemizeAllAsyncDelegate<MasterType> itemizeAllMasterDataAsyncDelegate,
            IConfirmAsyncDelegate<long> confirmDetailDataContainsIdAsyncDelegate)
        {
            this.itemizeAllMasterDataAsyncDelegate = itemizeAllMasterDataAsyncDelegate;
            this.confirmDetailDataContainsIdAsyncDelegate = confirmDetailDataContainsIdAsyncDelegate;
        }

        public async IAsyncEnumerable<MasterType> ItemizeAllAsync()
        {
            await foreach (var masterDataValue in itemizeAllMasterDataAsyncDelegate.ItemizeAllAsync())
                if (!await confirmDetailDataContainsIdAsyncDelegate.ConfirmAsync(masterDataValue.Id))
                    yield return masterDataValue;
        }
    }
}