using System.Threading.Tasks;

namespace Interfaces.Controllers.SerializedStorage
{
    public interface ISerializePushAsyncDelegate<T>
    {
        Task SerializePushAsync(string uri, T data);
    }

    public interface IDeserializePullAsyncDelegate<T>
    {
        Task<T> DeserializePullAsync(string uri);
    }

    public interface ISerializedStorageController<T>:
        IDeserializePullAsyncDelegate<T>,
        ISerializePushAsyncDelegate<T>
    {
        // ...
    }
}
