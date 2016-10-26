using System.Threading.Tasks;

namespace Interfaces.SerializedStorage
{
    public interface ISerializePushDelegate
    {
        Task SerializePush<T>(string uri, T data);
    }

    public interface IDeserializePullDelegate
    {
        Task<T> DeserializePull<T>(string uri);
    }

    public interface ISerializedStorageController:
        IDeserializePullDelegate,
        ISerializePushDelegate
    {
        // ...
    }
}
