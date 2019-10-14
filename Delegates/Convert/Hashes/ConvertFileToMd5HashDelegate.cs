using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Storage;
using Interfaces.Status;

namespace Delegates.Convert.Hashes
{
    public class ConvertFileToMd5HashDelegate: IConvertAsyncDelegate<string, Task<string>>
    {
        readonly IStorageController<string> storageController;
        readonly IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate;

        public ConvertFileToMd5HashDelegate(
            IStorageController<string> storageController,
            IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate)
        {
            this.storageController = storageController;
            this.convertStringToHashDelegate = convertStringToHashDelegate;
        }

        public async Task<string> ConvertAsync(string uri, IStatus status)
        {
            var fileContent = await storageController.PullAsync(uri);
            var fileHash = await convertStringToHashDelegate.ConvertAsync(fileContent, status);

            return fileHash;
        }
    }
}
