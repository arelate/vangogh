using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

using Models.ProductRecords;

namespace Controllers.Records
{
    public class RecordsController: IRecordsController
    {
        private IDataController<ProductRecords> productRecordsController;
        private IStatusController statusController;

        public RecordsController(
            IDataController<ProductRecords> productRecordsController,
            IStatusController statusController)
        {
            this.productRecordsController = productRecordsController;
            this.statusController = statusController;
        }

        public async Task<DateTime> GetRecordAsync(long id, RecordsTypes recordType, IStatus status)
        {
            var minRecord = DateTime.MinValue.ToUniversalTime();
            var productRecord = await productRecordsController.GetByIdAsync(id, status);

            if (productRecord == null || 
                productRecord.Records == null) return minRecord;

            return productRecord.Records.ContainsKey(recordType) ?
                productRecord.Records[recordType] :
                minRecord;
        }

        public async Task SetRecordAsync(long id, RecordsTypes recordType, IStatus status)
        {
            var productRecord = await productRecordsController.GetByIdAsync(id, status);

            if (productRecord == null)
            {
                await productRecordsController.CreateAsync(
                    status,
                    new ProductRecords
                    {
                        Records = new Dictionary<RecordsTypes, DateTime>()
                    });
                productRecord = await productRecordsController.GetByIdAsync(id, status);
            }

            var nowTimestamp = DateTime.UtcNow;

            if (!productRecord.Records.ContainsKey(recordType))
                productRecord.Records.Add(recordType, nowTimestamp);
            else productRecord.Records[recordType] = nowTimestamp;

            await productRecordsController.UpdateAsync(status, productRecord);
        }
    }
}
