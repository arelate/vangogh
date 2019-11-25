using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class ProductsRecordsDataController : DataController<ProductRecords>
    {
        public ProductsRecordsDataController(
            IStashController<Dictionary<long, ProductRecords>> productsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IStatusController statusController) :
            base(
                productsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                statusController)
        {
            // ...
        }
    }
}