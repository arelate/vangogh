using System;
using System.Threading.Tasks;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Interfaces.Controllers.Records
{
    public interface IGetRecordAsyncDelegate
    {
        Task<DateTime> GetRecordAsync(long id, RecordsTypes recordType, IStatus status);
    }

    public interface ISetRecordAsyncDelegate
    {
        Task SetRecordAsync(long id, RecordsTypes recordType, IStatus status);
    }

    public interface IRecordsController:
        IGetRecordAsyncDelegate,
        ISetRecordAsyncDelegate
    {
        // ...
    }
}
