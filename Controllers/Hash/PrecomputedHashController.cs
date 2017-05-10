using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Hash;
using Interfaces.Data;
using Interfaces.Destination.Filename;
using Interfaces.Serialization;
using Interfaces.Storage;

namespace Controllers.Hash
{
    public class PrecomputedHashController : 
        IPrecomputedHashController, 
        ILoadAsyncDelegate,
        ISaveAsyncDelegate
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private string uriHashesFilename;
        private bool dataLoaded;

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
            dataLoaded = false;
        }

        public string GetHash(string uri)
        {
            return (uriHashes.ContainsKey(uri)) ?
                uriHashes[uri] :
                string.Empty;
        }

        public async Task LoadAsync()
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

            dataLoaded = true;
        }

        public async Task SaveAsync()
        {
            if (!dataLoaded)
                throw new InvalidOperationException(
                    "Saving hashes without loading them first would overwrite existing data");

            var serialiedData = serializationController.Serialize(uriHashes);
            await transactionalStorageController.PushAsync(uriHashesFilename, serialiedData);
        }

        public async Task SetHashAsync(string uri, string hash)
        {
            if (uriHashes.ContainsKey(uri) &&
                uriHashes[uri] == hash) return;

            if (!uriHashes.ContainsKey(uri))
                uriHashes.Add(uri, hash);
            else uriHashes[uri] = hash;

            await SaveAsync();
        }
    }
}
