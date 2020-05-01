using System;
using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Models.RecordTypes;
using Models.ProductTypes;

namespace Delegates.Data.Records
{
    public class GetRecordDataAsyncDelegate : IGetDataAsyncDelegate<DateTime, (long Id, RecordsTypes RecordType)>
    {
        private readonly IGetDataAsyncDelegate<ProductRecords, long> getDataByIdAsyncDelegate;

        public GetRecordDataAsyncDelegate(
            IGetDataAsyncDelegate<ProductRecords, long> getDataByIdAsyncDelegate)
        {
            this.getDataByIdAsyncDelegate = getDataByIdAsyncDelegate;
        }

        public async Task<DateTime> GetDataAsync((long Id, RecordsTypes RecordType) idRecordType)
        {
            var minRecord = DateTime.MinValue.ToUniversalTime();
            var productRecord = await getDataByIdAsyncDelegate.GetDataAsync(idRecordType.Id);

            if (productRecord == null ||
                productRecord.Records == null) return minRecord;

            return productRecord.Records.ContainsKey(idRecordType.RecordType)
                ? productRecord.Records[idRecordType.RecordType]
                : minRecord;
        }
    }
}