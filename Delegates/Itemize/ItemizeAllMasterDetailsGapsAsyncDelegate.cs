using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductCore;

namespace Delegates.EnumerateIds
{
    public class ItemizeAllMasterDetailsGapsAsyncDelegate<MasterType, DetailType> :
        IItemizeAllAsyncDelegate<MasterType>
        where MasterType : ProductCore
        where DetailType : ProductCore
    {
        readonly IDataController<MasterType> masterDataController;
        readonly IDataController<DetailType> detailDataController;

        public ItemizeAllMasterDetailsGapsAsyncDelegate(
            IDataController<MasterType> masterDataController,
            IDataController<DetailType> detailDataController)
        {
            this.masterDataController = masterDataController;
            this.detailDataController = detailDataController;
        }

        public async IAsyncEnumerable<MasterType> ItemizeAllAsync(IStatus status)
        {
            await foreach (var masterDataValue in masterDataController.ItemizeAllAsync(status))
                if (!await detailDataController.ContainsIdAsync(masterDataValue.Id, status))
                    yield return masterDataValue;
        }
    }
}
