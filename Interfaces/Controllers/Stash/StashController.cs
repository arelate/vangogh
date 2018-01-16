using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Controllers.Stash
{
    public interface IDataAvailableDelegate
    {
        bool DataAvailable { get; }
    }

    public interface ILoadAsyncDelegate
    {
        Task LoadAsync(IStatus status);
    }

    public interface ISaveAsyncDelegate
    {
        Task SaveAsync(IStatus status);
    }

    public interface IGetDataAsyncDelegate<InterfaceType>
    {
        Task<InterfaceType> GetDataAsync(IStatus status);
    }

    public interface IStashController<InterfaceType, InstanceType>:
        IDataAvailableDelegate,
        IGetDataAsyncDelegate<InterfaceType>,
        ISaveAsyncDelegate,
        ILoadAsyncDelegate
    {
        // ...
    }
}
