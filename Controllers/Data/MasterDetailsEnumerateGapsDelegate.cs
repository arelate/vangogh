using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.ProductCore;

namespace Controllers.Data
{
    public class MasterDetailsEnumerateGapsDelegate<MasterType, DetailType>:
        IEnumerateIdsAsyncDelegate
        where MasterType: ProductCore
        where DetailType: ProductCore
    {
        private IDataController<MasterType> masterDataController;
        private IDataController<DetailType> detailDataController;

        public MasterDetailsEnumerateGapsDelegate(
            IDataController<MasterType> masterDataController,
            IDataController<DetailType> detailDataController)
        {
            this.masterDataController = masterDataController;
            this.detailDataController = detailDataController;
        }

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            var gaps = new List<long>();

            foreach (var id in await masterDataController.EnumerateIdsAsync(status))
                if (!await detailDataController.ContainsIdAsync(id, status))
                    gaps.Add(id);

            return gaps;
        }
    }
}
