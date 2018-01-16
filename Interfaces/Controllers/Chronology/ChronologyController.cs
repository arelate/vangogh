using System;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Interfaces.Controllers.Chronology
{
    public interface IGetCreatedAsyncDelegate<IdentityType>
    {
        Task<DateTime> GetCreatedAsync(IdentityType id, IStatus status);
    }

    public interface ISetCreatedAsyncDelegate<IdentityType>
    {
        Task SetCreatedAsync(IdentityType id, IStatus status);
    }

    public interface IGetUpdatedAsyncDelegate<IdentityType>
    {
        Task<DateTime> GetUpdatedAsync(IdentityType id, IStatus status);
    }

    public interface ISetUpdatedAsyncDelegate<IdentityType>
    {
        Task SetUpdatedAsync(IdentityType id, IStatus status);
    }

    public interface IGetCompletedAsyncDelegate<IdentityType>
    {
        Task<DateTime> GetCompletedAsync(IdentityType id, IStatus status);
    }

    public interface ISetCompletedAsyncDelegate<IdentityType>
    {
        Task SetCompletedAsync(IdentityType id, IStatus status);
    }

    public interface IChronologyController<IdentityType>:
        IGetCreatedAsyncDelegate<IdentityType>,
        ISetCreatedAsyncDelegate<IdentityType>,
        IGetUpdatedAsyncDelegate<IdentityType>,
        ISetUpdatedAsyncDelegate<IdentityType>,
        IGetCompletedAsyncDelegate<IdentityType>,
        ISetCompletedAsyncDelegate<IdentityType>
    {
        // ...
    }
}
