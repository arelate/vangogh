using System;
using System.Threading.Tasks;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Interfaces.Controllers.Records
{
    public interface IGetRecordAsyncDelegate<Type>
    {
        Task<DateTime> GetRecordAsync(Type id, RecordsTypes recordType, IStatus status);
    }

    public interface ISetRecordAsyncDelegate<Type>
    {
        Task SetRecordAsync(Type id, RecordsTypes recordType, IStatus status);
    }

    public interface IRecordsController<Type>:
        IGetRecordAsyncDelegate<Type>,
        ISetRecordAsyncDelegate<Type>
    {
        // ...
    }
}
