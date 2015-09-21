using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG
{
    public class SaveLoadDataHelper
    {
        private IStorageController<string> storageController;
        private IStringifyController stringifyController;

        public SaveLoadDataHelper(
            IStorageController<string> storageController,
            IStringifyController stringifyController)
        {
            this.storageController = storageController;
            this.stringifyController = stringifyController;
        }

        public async Task<T> LoadData<T>(
            string uri,
            string replaceString)
        {
            try
            {
                var data = await storageController.Pull(uri);

                if (replaceString != string.Empty)
                    data = data.Replace(replaceString, string.Empty);

                return stringifyController.Parse<T>(data);
            }
            catch
            {
                return default(T);
            }
        }

        public async Task SaveData<T>(
            string uri,
            T data,
            string replaceString)
        {
            try
            {
                var stringData = stringifyController.Stringify(data);

                if (replaceString != string.Empty)
                    stringData = replaceString + stringData;

                await storageController.Push(uri, stringData);
            }
            catch
            {
                // ...
            }
        }

    }
}
