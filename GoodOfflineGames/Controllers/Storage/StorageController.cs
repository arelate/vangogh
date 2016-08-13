using System.Threading.Tasks;
using System.IO;

using Interfaces.Stream;
using Interfaces.File;
using Interfaces.Storage;

namespace Controllers.Storage
{
    public class StorageController: IStringStorageController
    {
        private IStreamController streamController;
        private IFileController fileController;

        public StorageController(
            IStreamController streamController,
            IFileController fileController)
        {
            this.streamController = streamController;
            this.fileController = fileController;
        }

        public async Task Push(
            string uri,
            string data)
        {
            using (var stream = streamController.OpenWritable(uri))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteLineAsync(data.ToString());
        }

        public async Task<string> Pull(string uri)
        {
            var data = string.Empty;

            if (!fileController.Exists(uri)) return data;

            using (var stream = streamController.OpenReadable(uri))
                using (StreamReader reader = new StreamReader(stream))
                    data = await reader.ReadToEndAsync();

            return data;
        }
    }
}
