using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Hash;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

namespace Controllers.Hash
{
    public class StoredHashController : IStoredHashController
    {
        readonly IStashController<Dictionary<string, string>> precomputedHashesStashController;

        public StoredHashController(
            IStashController<Dictionary<string, string>> precomputedHashesStashController)
        {
            this.precomputedHashesStashController = precomputedHashesStashController;
        }

        public async Task<IEnumerable<string>> ItemizeAllAsync(IStatus status)
        {
            var storedHashes = await precomputedHashesStashController.GetDataAsync(status);

            return storedHashes.Keys;
        }

        public async Task<string> GetHashAsync(string uri, IStatus status)
        {
            var storedHashes = await precomputedHashesStashController.GetDataAsync(status);

            return (storedHashes.ContainsKey(uri)) ?
                storedHashes[uri] :
                string.Empty;
        }

        public async Task SetHashAsync(string uri, string hash, IStatus status)
        {
            var storedHashes = await precomputedHashesStashController.GetDataAsync(status);

            if (storedHashes.ContainsKey(uri) &&
                storedHashes[uri] == hash) return;

            if (!storedHashes.ContainsKey(uri))
                storedHashes.Add(uri, hash);
            else storedHashes[uri] = hash;

            await precomputedHashesStashController.SaveAsync(status);
        }
    }
}
