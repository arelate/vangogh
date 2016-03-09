using System.Threading.Tasks;
using System.IO;

using GOG.Interfaces;
using System;

namespace GOG.SharedControllers
{
    public class StorageController: IStorageController<string>
    {
        private IIOController ioController;

        public StorageController(IIOController ioController)
        {
            this.ioController = ioController;
        }

        public async Task Push(
            string uri,
            string data)
        {
            using (var stream = ioController.OpenWritable(uri))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    await writer.WriteLineAsync(data);
                }
            }
        }

        public async Task<string> Pull(
            string uri)
        {
            string data = string.Empty;

            if (!ioController.FileExists(uri)) return data;

            using (var stream = ioController.OpenReadable(uri))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    data = await reader.ReadToEndAsync();
                }
            }

            return data;
        }
    }
}
