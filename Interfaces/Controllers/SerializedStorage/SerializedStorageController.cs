using System.Threading.Tasks;

namespace Interfaces.Controllers.SerializedStorage
{
    public interface ISerializePushAsyncDelegate
    {
        Task SerializePushAsync<T>(string uri, T data);
    }

    public interface IDeserializePullAsyncDelegate
    {
        Task<T> DeserializePullAsync<T>(string uri);
    }

    public interface ISerializedStorageController:
        IDeserializePullAsyncDelegate,
        ISerializePushAsyncDelegate
    {
        // ...
    }
}
