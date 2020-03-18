using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;


namespace Controllers.Stash
{
    public abstract class StashController<T> : IStashController<T> where T : class, new()
    {
        readonly IGetPathDelegate getPathDelegate;
        readonly ISerializedStorageController serializedStorageController;
        readonly IActionLogController actionLogController;

        // T storedData;

        public StashController(
            IGetPathDelegate getPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController)
        {
            this.getPathDelegate = getPathDelegate;
            this.serializedStorageController = serializedStorageController;
            this.actionLogController = actionLogController;
        }

        T storedData;

        public async Task<T> GetDataAsync(string uri = "")
        {
            if (storedData == null)
            {
                actionLogController.StartAction("Load stored data");

                var storedDataUri = getPathDelegate.GetPath(string.Empty, string.Empty);

                storedData = await serializedStorageController.DeserializePullAsync<T>(storedDataUri);

                if (storedData == null) storedData = new T();

                // DataAvailable = true;

                actionLogController.CompleteAction();
            }

            return storedData;
        }


        public async Task PostDataAsync(T data, string uri)
        {
            actionLogController.StartAction("Save stored data");

            var storedDataUri = getPathDelegate.GetPath(string.Empty, string.Empty);

            await serializedStorageController.SerializePushAsync(storedDataUri, data);

            actionLogController.CompleteAction();
        }
    }
}
