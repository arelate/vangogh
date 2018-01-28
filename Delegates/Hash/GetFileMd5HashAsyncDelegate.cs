using System.Threading.Tasks;

using Interfaces.Delegates.Hash;

using Interfaces.Controllers.Storage;
using Interfaces.Status;

namespace Delegates.Hash
{
    public class GetFileMd5HashAsyncDelegate: IGetHashAsyncDelegate<string>
    {
        private IStorageController<string> storageController;
        private IGetHashAsyncDelegate<string> getStringHashAsyncDelegate;

        public GetFileMd5HashAsyncDelegate(
            IStorageController<string> storageController,
            IGetHashAsyncDelegate<string> getStringHashAsyncDelegate)
        {
            this.storageController = storageController;
            this.getStringHashAsyncDelegate = getStringHashAsyncDelegate;
        }

        public async Task<string> GetHashAsync(string uri, IStatus status)
        {
            var fileContent = await storageController.PullAsync(uri);
            var fileHash = await getStringHashAsyncDelegate.GetHashAsync(fileContent, status);

            return fileHash;
        }
    }
}
