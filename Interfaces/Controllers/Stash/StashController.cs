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

    public interface IGetDataAsyncDelegate<ModelType>
    {
        Task<ModelType> GetDataAsync(IStatus status);
    }

    public interface IStashController<ModelType>:
        IDataAvailableDelegate,
        IGetDataAsyncDelegate<ModelType>,
        ISaveAsyncDelegate,
        ILoadAsyncDelegate
    {
        // ...
    }
}
