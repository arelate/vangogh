using System;
using System.Threading.Tasks;
using Interfaces.Controllers.Records;
using Interfaces.Models.RecordsTypes;
using Interfaces.Status;

namespace Controllers.Records
{
    public class ActivityRecordsController: IRecordsController<string>
    {
        private IRecordsController<long> identityRecordsController;

        public ActivityRecordsController(IRecordsController<long> identityRecordsController)
        {
            this.identityRecordsController = identityRecordsController;
        }

        public async Task<DateTime> GetRecordAsync(string activity, RecordsTypes recordType, IStatus status)
        {
            var id = activity.GetHashCode();
            return await identityRecordsController.GetRecordAsync(id, recordType, status);
        }

        public async Task SetRecordAsync(string activity, RecordsTypes recordType, IStatus status)
        {
            var id = activity.GetHashCode();
            await identityRecordsController.SetRecordAsync(id, recordType, status);
        }
    }
}
