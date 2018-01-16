using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.SerializedStorage;

using Interfaces.Controllers.Chronology;

using Interfaces.Status;

using Models.Chronology;

namespace Controllers.Chronology
{
    public class ChronologyController : IChronologyController<long>
    {
        private IDictionary<long, Events> productEvents;

        private IGetDirectoryAsyncDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private ISerializedStorageController serializedStorageController;

        private IStatusController statusController;

        public ChronologyController(
            IGetDirectoryAsyncDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.serializedStorageController = serializedStorageController;

            this.statusController = statusController;
        }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task<DateTime> GetCompletedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (productEvents.ContainsKey(id)) return productEvents[id].Completed;

            return DateTime.MinValue.ToUniversalTime();
        }

        public async Task<DateTime> GetCreatedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (productEvents.ContainsKey(id)) return productEvents[id].Created;

            return DateTime.MinValue.ToUniversalTime();
        }

        public async Task<DateTime> GetUpdatedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (productEvents.ContainsKey(id)) return productEvents[id].Updated;

            return DateTime.MinValue.ToUniversalTime();
        }

        // TODO: This is largely the same code as IndexController.Load, seems like DRY opportunity
        public Task LoadAsync(IStatus status)
        {
            //var loadStatus = await statusController.CreateAsync(status, "Load chronology");

            //var productEventsUri = Path.Combine(
            //    getDirectoryDelegate.GetDirectory(),
            //    getFilenameDelegate.GetFilename());

            //productEvents = await serializedStorageController.DeserializePullAsync<Dictionary<long, Events>>(productEventsUri, loadStatus);
            //if (productEvents == null) productEvents = new Dictionary<long, Events>();

            //DataAvailable = true;

            //await statusController.CompleteAsync(loadStatus);
            throw new NotImplementedException();
        }

        // TODO: This is largely the same code as IndexController.Save, seems like DRY opportunity
        public async Task SaveAsync(IStatus status)
        {
            if (!DataAvailable) throw new InvalidOperationException("Cannot save data before it's available");

            var saveStatus = await statusController.CreateAsync(status, "Save index");

            var productEventsUri = Path.Combine(
                await getDirectoryDelegate.GetDirectoryAsync(string.Empty, saveStatus),
                getFilenameDelegate.GetFilename());

            await serializedStorageController.SerializePushAsync(productEventsUri, productEvents, saveStatus);

            await statusController.CompleteAsync(saveStatus);
        }

        public async Task SetCompletedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (productEvents.ContainsKey(id)) productEvents[id].Completed = DateTime.UtcNow;
            else productEvents.Add(id, new Events() { Completed = DateTime.UtcNow, Updated = DateTime.UtcNow });
        }

        public async Task SetCreatedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (productEvents.ContainsKey(id)) productEvents[id].Created = DateTime.UtcNow;
            else productEvents.Add(id, new Events() { Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
        }

        public async Task SetUpdatedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (productEvents.ContainsKey(id)) productEvents[id].Created = DateTime.UtcNow;
            else productEvents.Add(id, new Events() { Updated = DateTime.UtcNow });
        }
    }
}
