using System.Threading.Tasks;
using System.IO;

using GOG.Interfaces;

namespace GOG.SharedControllers
{
    public class StorageController: IStorageController<string>
    {
        private IStreamController streamController;

        public StorageController(IStreamController streamController)
        {
            this.streamController = streamController;
        }

        public async Task Push(
            string uri,
            string data)
        {
            using (var stream = streamController.OpenWritable(uri))
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

            using (var stream = streamController.OpenReadable(uri))
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
