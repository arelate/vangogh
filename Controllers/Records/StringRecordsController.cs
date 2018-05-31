using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

namespace Controllers.Records
{
    public class StringRecordsController: IRecordsController<string>
    {
        readonly IRecordsController<long> indexRecordsController;
        readonly IConvertDelegate<string, long> convertStringToIndexDelegate;

        public StringRecordsController(
            IRecordsController<long> indexRecordsController,
            IConvertDelegate<string, long> convertStringToIndexDelegate)
        {
            this.indexRecordsController = indexRecordsController;
            this.convertStringToIndexDelegate = convertStringToIndexDelegate;
        }

        public async Task<DateTime> GetRecordAsync(string activity, RecordsTypes recordType, IStatus status)
        {
            var index = convertStringToIndexDelegate.Convert(activity);
            return await indexRecordsController.GetRecordAsync(index, recordType, status);
        }

        public async Task SetRecordAsync(string activity, RecordsTypes recordType, IStatus status)
        {
            var index = convertStringToIndexDelegate.Convert(activity);
            await indexRecordsController.SetRecordAsync(index, recordType, status);
        }
    }
}
