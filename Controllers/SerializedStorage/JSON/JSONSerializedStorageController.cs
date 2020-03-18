using System.Threading.Tasks;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

namespace Controllers.SerializedStorage
{
    public abstract class JSONSerializedStorageController<T> : ISerializedStorageController<T>
    {
        readonly IStorageController<string> storageController;
        private readonly IConvertDelegate<T, string> convertTypeToJSONDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;

        IActionLogController actionLogController;

        public JSONSerializedStorageController(
            IStorageController<string> storageController,
            IConvertDelegate<T, string> convertTypeToJSONDelegate,
            IConvertDelegate<string, T> convertJSONToTypeDelegate,
            IActionLogController actionLogController)
        {
            this.storageController = storageController;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;
            this.convertTypeToJSONDelegate = convertTypeToJSONDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task<T> DeserializePullAsync(string uri)
        {
            var serializedData = await storageController.PullAsync(uri);
            return convertJSONToTypeDelegate.Convert(serializedData);
        }

        public async Task SerializePushAsync(string uri, T data)
        {
            var serializedData = convertTypeToJSONDelegate.Convert(data);
            await storageController.PushAsync(uri, serializedData);
        }
    }
}
