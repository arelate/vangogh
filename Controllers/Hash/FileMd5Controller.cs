using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Controllers.Hash;

using Interfaces.Storage;

namespace Controllers.Hash
{
    public class FileMd5Controller : IFileHashController
    {
        private IStorageController<string> storageController;
        private IStringHashController stringHashController;

        public FileMd5Controller(
            IStorageController<string> storageController,
            IStringHashController stringHashController)
        {
            this.storageController = storageController;
            this.stringHashController = stringHashController;
        }

        public async Task<string> GetHashAsync(string uri)
        {
            var fileContent = await storageController.PullAsync(uri);
            var fileHash = stringHashController.GetHash(fileContent);

            return fileHash;
        }
    }
}
