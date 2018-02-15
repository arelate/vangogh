using System;
using System.Threading.Tasks;
using Interfaces.Controllers.Records;
using Interfaces.Models.RecordsTypes;
using Interfaces.Status;

namespace Controllers.Records
{
    public class StringRecordsController: IRecordsController<string>
    {
        private IRecordsController<long> indexRecordsController;

        public StringRecordsController(IRecordsController<long> indexRecordsController)
        {
            this.indexRecordsController = indexRecordsController;
        }

        public async Task<DateTime> GetRecordAsync(string activity, RecordsTypes recordType, IStatus status)
        {
            var hash = activity.GetHashCode();
            return await indexRecordsController.GetRecordAsync(hash, recordType, status);
        }

        public async Task SetRecordAsync(string activity, RecordsTypes recordType, IStatus status)
        {
            var hash = activity.GetHashCode();
            await indexRecordsController.SetRecordAsync(hash, recordType, status);
        }
    }
}
