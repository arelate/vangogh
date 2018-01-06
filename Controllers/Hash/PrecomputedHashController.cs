using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Hash;
using Interfaces.Controllers.Data;
using Interfaces.Destination.Filename;
using Interfaces.Serialization;
using Interfaces.Storage;
using Interfaces.Status;

namespace Controllers.Hash
{
    public class PrecomputedHashController : IPrecomputedHashController
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private string uriHashesFilename;

        private ISerializationController<string> serializationController;
        private ITransactionalStorageController transactionalStorageController;
        private IDictionary<string, string> uriHashes;

        public PrecomputedHashController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializationController<string> serializationController,
            ITransactionalStorageController transactionalStorageController)
        {
            this.getFilenameDelegate = getFilenameDelegate;

            uriHashesFilename = getFilenameDelegate.GetFilename();

            this.serializationController = serializationController;
            this.transactionalStorageController = transactionalStorageController;

            uriHashes = new Dictionary<string, string>();
        }

        public async Task<IEnumerable<string>> EnumerateKeysAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return uriHashes.Keys;
        }

        public string GetHash(string uri)
        {
            if (!DataAvailable) LoadAsync().Wait();

            return (uriHashes.ContainsKey(uri)) ?
                uriHashes[uri] :
                string.Empty;
        }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task LoadAsync(IStatus status = null)
        {
            if (uriHashes != null &&
                uriHashes.Any())
                throw new InvalidOperationException(
                    "Loading hashes when there is data already would lead to overwrite of the existing data");

            try
            {
                var serializedData = await transactionalStorageController.PullAsync(uriHashesFilename);
                uriHashes = serializationController.Deserialize<Dictionary<string, string>>(serializedData);
            }
            catch
            {
                uriHashes = null;
            }

            if (uriHashes == null)
                uriHashes = new Dictionary<string, string>();

            DataAvailable = true;
        }

        public async Task SaveAsync(IStatus status = null)
        {
            if (!DataAvailable)
                throw new InvalidOperationException(
                    "Saving hashes without loading them first would overwrite existing data");

            var serialiedData = serializationController.Serialize(uriHashes);
            await transactionalStorageController.PushAsync(uriHashesFilename, serialiedData);
        }

        public async Task SetHashAsync(string uri, string hash, IStatus status)
        {
            if (uriHashes.ContainsKey(uri) &&
                uriHashes[uri] == hash) return;

            if (!uriHashes.ContainsKey(uri))
                uriHashes.Add(uri, hash);
            else uriHashes[uri] = hash;

            await SaveAsync(status);
        }
    }
}
