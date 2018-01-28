using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Controllers.SerializedStorage
{
    public interface ISerializePushAsyncDelegate
    {
        Task SerializePushAsync<T>(string uri, T data, IStatus status);
    }

    public interface IDeserializePullAsyncDelegate
    {
        Task<T> DeserializePullAsync<T>(string uri, IStatus status);
    }

    public interface ISerializedStorageController:
        IDeserializePullAsyncDelegate,
        ISerializePushAsyncDelegate
    {
        // ...
    }
}
