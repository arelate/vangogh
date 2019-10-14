using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Hashes;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

namespace Controllers.Hashes
{
    public class HashesController : IHashesController
    {
        readonly IStashController<Dictionary<string, string>> hashesStashController;

        public HashesController(
            IStashController<Dictionary<string, string>> hashesStashController)
        {
            this.hashesStashController = hashesStashController;
        }

        public async Task<IEnumerable<string>> ItemizeAllAsync(IStatus status)
        {
            var storedHashes = await hashesStashController.GetDataAsync(status);

            return storedHashes.Keys;
        }

        public async Task<string> ConvertAsync(string uri, IStatus status)
        {
            var storedHashes = await hashesStashController.GetDataAsync(status);

            return (storedHashes.ContainsKey(uri)) ?
                storedHashes[uri] :
                string.Empty;
        }

        public async Task SetHashAsync(string uri, string hash, IStatus status)
        {
            var storedHashes = await hashesStashController.GetDataAsync(status);

            if (storedHashes.ContainsKey(uri) &&
                storedHashes[uri] == hash) return;

            if (!storedHashes.ContainsKey(uri))
                storedHashes.Add(uri, hash);
            else storedHashes[uri] = hash;
        }

        public async Task CommitAsync(IStatus status)
        {
            await hashesStashController.SaveAsync(status);
        }
    }
}
