using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Models.RecordsTypes;

using Models.ProductTypes;

namespace Controllers.Records
{
    public class IndexRecordsController : IRecordsController<long>
    {
        readonly IDataController<ProductRecords> productRecordsController;
        readonly IActionLogController actionLogController;

        public IndexRecordsController(
            IDataController<ProductRecords> productRecordsController,
            IActionLogController actionLogController)
        {
            this.productRecordsController = productRecordsController;
            this.actionLogController = actionLogController;
        }

        public async Task CommitAsync()
        {
            await productRecordsController.CommitAsync();
        }

        public async Task<DateTime> GetRecordAsync(long id, RecordsTypes recordType)
        {
            var minRecord = DateTime.MinValue.ToUniversalTime();
            var productRecord = await productRecordsController.GetByIdAsync(id);

            if (productRecord == null ||
                productRecord.Records == null) return minRecord;

            return productRecord.Records.ContainsKey(recordType) ?
                productRecord.Records[recordType] :
                minRecord;
        }

        public async Task SetRecordAsync(long id, RecordsTypes recordType)
        {
            var productRecord = await productRecordsController.GetByIdAsync(id);

            if (productRecord == null)
                productRecord = new ProductRecords
                {
                    Id = id,
                    Records = new Dictionary<RecordsTypes, DateTime>()
                };

            var nowTimestamp = DateTime.UtcNow;

            if (!productRecord.Records.ContainsKey(recordType))
                productRecord.Records.Add(recordType, nowTimestamp);
            else productRecord.Records[recordType] = nowTimestamp;

            await productRecordsController.UpdateAsync(productRecord);
        }
    }
}
