using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GOG.Interfaces;

namespace GOG
{
    public class SaveLoadDataHelper: ISaveLoadDataController
    {
        private IStorageController<string> storageController;
        private ISerializationController<string> stringifyController;
        private Dictionary<ProductTypes, string> filenames;
        private Dictionary<ProductTypes, string> prefixes;

        public SaveLoadDataHelper(
            IStorageController<string> storageController,
            ISerializationController<string> stringifyController,
            Dictionary<ProductTypes, string> filenames,
            Dictionary<ProductTypes, string> prefixes)
        {
            this.storageController = storageController;
            this.stringifyController = stringifyController;
            this.filenames = filenames;
            this.prefixes = prefixes;
        }

        public async Task<T> LoadData<T>(ProductTypes type)
        {
            try
            {
                var data = await storageController.Pull(filenames[type]);

                if (prefixes[type] != string.Empty)
                    data = data.Replace(prefixes[type], string.Empty);

                return stringifyController.Deserialize<T>(data);
            }
            catch
            {
                return default(T);
            }
        }

        public async Task SaveData<T>(T data, ProductTypes type)
        {
            try
            {
                var stringData = stringifyController.Serialize(data);

                if (prefixes[type] != string.Empty)
                    stringData = prefixes[type] + stringData;

                await storageController.Push(filenames[type], stringData);
            }
            catch (SerializationException)
            {
                // ...
            }
            catch (IOException)
            {
                // ...
            }
        }

    }
}
