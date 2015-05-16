using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GOG
{
    public class Storage
    {
        private IStreamController streamController;

        public Storage(IStreamController streamController)
        {
            this.streamController = streamController;
        }

        public async Task Put(
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
