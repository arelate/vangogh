using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Interfaces.Models.RecordTypes;
using Models.ProductTypes;

namespace Delegates.Data.Records
{
    public class PostRecordDataAsyncDelegate: IPostDataAsyncDelegate<(long Id, RecordsTypes RecordType)>
    {
        private readonly IGetDataAsyncDelegate<ProductRecords, long> getDataByIdAsyncDelegate;
        private readonly IUpdateAsyncDelegate<ProductRecords> updateProductRecordsAsyncDelegate;

        public PostRecordDataAsyncDelegate(
            IGetDataAsyncDelegate<ProductRecords, long> getDataByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductRecords> updateProductRecordsAsyncDelegate)
        {
            this.getDataByIdAsyncDelegate = getDataByIdAsyncDelegate;
            this.updateProductRecordsAsyncDelegate = updateProductRecordsAsyncDelegate;
        }
        
        public async Task<string> PostDataAsync((long Id, RecordsTypes RecordType) idRecordType, string uri = null)
        {
            var productRecord = await getDataByIdAsyncDelegate.GetDataAsync(idRecordType.Id);

            if (productRecord == null)
                productRecord = new ProductRecords
                {
                    Id = idRecordType.Id,
                    Records = new Dictionary<RecordsTypes, DateTime>()
                };

            var nowTimestamp = DateTime.UtcNow;

            if (!productRecord.Records.ContainsKey(idRecordType.RecordType))
                productRecord.Records.Add(idRecordType.RecordType, nowTimestamp);
            else productRecord.Records[idRecordType.RecordType] = nowTimestamp;

            await updateProductRecordsAsyncDelegate.UpdateAsync(productRecord);

            return uri;
        }
    }
}