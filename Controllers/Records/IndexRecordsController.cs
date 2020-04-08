using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Models.RecordsTypes;
using Models.ProductTypes;

namespace Controllers.Records
{
    public abstract class IndexRecordsController : IRecordsController<long>
    {
        private readonly IDataController<ProductRecords> productRecordsController;

        public IndexRecordsController(
            IDataController<ProductRecords> productRecordsController)
        {
            this.productRecordsController = productRecordsController;
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

            return productRecord.Records.ContainsKey(recordType) ? productRecord.Records[recordType] : minRecord;
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