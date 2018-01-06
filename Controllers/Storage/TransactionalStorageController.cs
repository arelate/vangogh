using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Controllers.File;

using Interfaces.Storage;

namespace Controllers.Storage {
    public class TransactionalStorageController: ITransactionalStorageController
    {
        private IStorageController<string> storageController; 
        private IFileController fileController;

        public TransactionalStorageController(
            IStorageController<string> storageController,
            IFileController fileController)
        {
            this.storageController = storageController;
            this.fileController = fileController;
        }

        public Task<string> PullAsync(string uri)
        {
            return storageController.PullAsync(uri);
        }

        public async Task PushAsync(string uri, string data)
        {
            var temporaryUri = Path.GetTempFileName();
            await storageController.PushAsync(temporaryUri, data);
            fileController.Move(temporaryUri, uri);
        }
    }
}