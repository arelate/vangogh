using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Records
{
    public class IndexRecordsController : IRecordsController<long>
    {
        readonly IDataController<ProductRecords> productRecordsDataController;
        IStatusController statusController;

        public IndexRecordsController(
            IDataController<ProductRecords> productRecordsController,
            IStatusController statusController)
        {
            this.productRecordsDataController = productRecordsController;
            this.statusController = statusController;
        }

        public async Task<DateTime> GetRecordAsync(long id, RecordsTypes recordType, IStatus status)
        {
            var minRecord = DateTime.MinValue.ToUniversalTime();
            var productRecord = await productRecordsDataController.GetByIdAsync(id, status);

            if (productRecord == null ||
                productRecord.Records == null) return minRecord;

            return productRecord.Records.ContainsKey(recordType) ?
                productRecord.Records[recordType] :
                minRecord;
        }

        public async Task SetRecordAsync(long id, RecordsTypes recordType, IStatus status)
        {
            var productRecord = await productRecordsDataController.GetByIdAsync(id, status);

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

            await productRecordsDataController.UpdateAsync(productRecord, status);
        }
    }
}
