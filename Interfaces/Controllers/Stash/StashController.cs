using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Controllers.Stash
{
    // public interface IDataAvailableDelegate
    // {
    //     bool DataAvailable { get; }
    // }

    // public interface ILoadAsyncDelegate
    // {
    //     Task LoadAsync();
    // }

    public interface IPostDataAsyncDelegate
    {
        Task PostDataAsync();
    }

    public interface IGetDataAsyncDelegate<ModelType>
    {
        Task<ModelType> GetDataAsync();
    }

    public interface IStashController<ModelType>:
        // IDataAvailableDelegate,
        IGetDataAsyncDelegate<ModelType>,
        IPostDataAsyncDelegate
        // ILoadAsyncDelegate
    {
        // ...
    }
}
