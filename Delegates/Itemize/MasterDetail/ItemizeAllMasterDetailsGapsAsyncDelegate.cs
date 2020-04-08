using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Controllers.Data;
using Models.ProductTypes;

namespace Delegates.Itemize.MasterDetail
{
    public abstract class ItemizeAllMasterDetailsGapsAsyncDelegate<MasterType, DetailType> :
        IItemizeAllAsyncDelegate<MasterType>
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        private readonly IDataController<MasterType> masterDataController;
        private readonly IDataController<DetailType> detailDataController;

        public ItemizeAllMasterDetailsGapsAsyncDelegate(
            IDataController<MasterType> masterDataController,
            IDataController<DetailType> detailDataController)
        {
            this.masterDataController = masterDataController;
            this.detailDataController = detailDataController;
        }

        public async IAsyncEnumerable<MasterType> ItemizeAllAsync()
        {
            await foreach (var masterDataValue in masterDataController.ItemizeAllAsync())
                if (!await detailDataController.ContainsIdAsync(masterDataValue.Id))
                    yield return masterDataValue;
        }
    }
}