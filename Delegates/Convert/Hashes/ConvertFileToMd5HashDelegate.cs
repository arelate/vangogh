using System.Threading.Tasks;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Convert.Hashes
{
    public class ConvertFileToMd5HashDelegate : IConvertAsyncDelegate<string, Task<string>>
    {
        private readonly IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate;
        private readonly IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate;

        [Dependencies(
            typeof(Delegates.Data.Storage.GetStringDataAsyncDelegate),
            typeof(Delegates.Convert.Hashes.ConvertStringToMd5HashDelegate))]
        public ConvertFileToMd5HashDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate)
        {
            this.getStringDataAsyncDelegate = getStringDataAsyncDelegate;
            this.convertStringToHashDelegate = convertStringToHashDelegate;
        }

        public async Task<string> ConvertAsync(string uri)
        {
            var fileContent = await getStringDataAsyncDelegate.GetDataAsync(uri);
            var fileHash = await convertStringToHashDelegate.ConvertAsync(fileContent);

            return fileHash;
        }
    }
}