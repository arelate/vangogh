using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductCore;

namespace Delegates.EnumerateIds
{
    public class ItemizeAllMasterDetailsGapsAsyncDelegate<MasterType, DetailType>:
        IItemizeAllAsyncDelegate<long>
        where MasterType: ProductCore
        where DetailType: ProductCore
    {
        private IDataController<MasterType> masterDataController;
        private IDataController<DetailType> detailDataController;

        public ItemizeAllMasterDetailsGapsAsyncDelegate(
            IDataController<MasterType> masterDataController,
            IDataController<DetailType> detailDataController)
        {
            this.masterDataController = masterDataController;
            this.detailDataController = detailDataController;
        }

        public async Task<IEnumerable<long>> ItemizeAllAsync(IStatus status)
        {
            var gaps = new List<long>();

            foreach (var id in await masterDataController.ItemizeAllAsync(status))
                if (!await detailDataController.ContainsIdAsync(id, status))
                    gaps.Add(id);

            return gaps;
        }
    }
}
