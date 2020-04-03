using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Attributes;

namespace Delegates.Convert.Hashes
{
    public class ConvertFileToMd5HashDelegate: IConvertAsyncDelegate<string, Task<string>>
    {
        private readonly IGetDataAsyncDelegate<string> getStringDataAsyncDelegate;
        readonly IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate;

		[Dependencies(
			"Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
			"Delegates.Convert.Hashes.ConvertStringToMd5HashDelegate,Delegates")]
        public ConvertFileToMd5HashDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
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
