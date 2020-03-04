using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Records
{
    // TODO: Figure out better way to store strings and not convert from a number
    public abstract class StringRecordsController: IRecordsController<string>
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

        public async Task CommitAsync()
        {
            await indexRecordsController.CommitAsync();
        }

        public async Task<DateTime> GetRecordAsync(string activity, RecordsTypes recordType)
        {
            var index = convertStringToIndexDelegate.Convert(activity);
            return await indexRecordsController.GetRecordAsync(index, recordType);
        }

        public async Task SetRecordAsync(string activity, RecordsTypes recordType)
        {
            var index = convertStringToIndexDelegate.Convert(activity);
            await indexRecordsController.SetRecordAsync(index, recordType);
        }
    }
}
