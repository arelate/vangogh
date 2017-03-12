using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                throw new InvalidOperationException("Loading hashes when there is data already would lead to overwrite of the existing data");

            var serializedData = await storageController.PullAsync(uriHashesFilename);
            uriHashes = serializationController.Deserialize<Dictionary<string, int>>(serializedData);

            if (uriHashes == null)
                uriHashes = new Dictionary<string, int>();
        }

        public async Task SaveAsync()
        {
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
