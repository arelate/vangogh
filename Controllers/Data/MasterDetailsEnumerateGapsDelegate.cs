using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Data;

using Models.ProductCore;

namespace Controllers.Data
{
    public class MasterDetailsEnumerateGapsDelegate<MasterType, DetailType>:
        IEnumerateIdsDelegate
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

        public IEnumerable<long> EnumerateIds()
        {
            foreach (var id in masterDataController.EnumerateIds())
                if (!detailDataController.ContainsId(id))
                    yield return id;
        }
    }
}
