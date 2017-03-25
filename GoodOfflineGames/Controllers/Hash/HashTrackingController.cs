using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Hash;
using Interfaces.Data;
using Interfaces.Destination.Filename;
using Interfaces.Serialization;
using Interfaces.Storage;

namespace Controllers.Hash
{
    public class HashTrackingController : 
        IHashTrackingController, 
        ILoadDelegate,
        ISaveDelegate
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private string uriHashesFilename;
        private bool dataLoaded;

        private ISerializationController<string> serializationController;
        private IStorageController<string> storageController;
        private IDictionary<string, int> uriHashes;

        public HashTrackingController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController)
        {
            this.getFilenameDelegate = getFilenameDelegate;

            uriHashesFilename = getFilenameDelegate.GetFilename();

            this.serializationController = serializationController;
            this.storageController = storageController;

            uriHashes = new Dictionary<string, int>();
            dataLoaded = false;
        }

        public int GetHash(string uri)
        {
            return (uriHashes.ContainsKey(uri)) ?
                uriHashes[uri] :
                int.MinValue;
        }

        public async Task LoadAsync()
        {
            if (uriHashes != null &&
                uriHashes.Count > 0)
                throw new InvalidOperationException(
                    "Loading hashes when there is data already would lead to overwrite of the existing data");

            try
            {
                var serializedData = await storageController.PullAsync(uriHashesFilename);
                uriHashes = serializationController.Deserialize<Dictionary<string, int>>(serializedData);
            }
            catch
            {
                uriHashes = null;
            }

            if (uriHashes == null)
                uriHashes = new Dictionary<string, int>();

            dataLoaded = true;
        }

        public async Task SaveAsync()
        {
            if (!dataLoaded)
                throw new InvalidOperationException(
                    "Saving hashes without loading them first would overwrite existing data");

            var serialiedData = serializationController.Serialize(uriHashes);
            await storageController.PushAsync(uriHashesFilename, serialiedData);
        }

        public async Task SetHashAsync(string uri, int hash)
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
