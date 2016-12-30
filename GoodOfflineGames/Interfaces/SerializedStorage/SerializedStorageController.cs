using System.Threading.Tasks;

namespace Interfaces.SerializedStorage
{
    public interface ISerializePushDelegate
    {
        Task SerializePushAsync<T>(string uri, T data);
    }

    public interface IDeserializePullDelegate
    {
        Task<T> DeserializePullAsync<T>(string uri);
    }

    public interface ISerializedStorageController:
        IDeserializePullDelegate,
        ISerializePushDelegate
    {
        // ...
    }
}
