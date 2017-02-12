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
        private IDictionary<string, string> uriHashes;

        public HashTrackingController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController)
        {
            this.getFilenameDelegate = getFilenameDelegate;

            uriHashesFilename = getFilenameDelegate.GetFilename();

            this.serializationController = serializationController;
            this.storageController = storageController;

            uriHashes = new Dictionary<string, string>();
        }

        public string GetHash(string uri)
        {
            return (uriHashes.ContainsKey(uri)) ?
                uriHashes[uri] :
                string.Empty;
        }

        public async Task LoadAsync()
        {
            var serializedData = await storageController.PullAsync(uriHashesFilename);
            uriHashes = serializationController.Deserialize<Dictionary<string, string>>(serializedData);
        }

        public async Task SaveAsync()
        {
            var serialiedData = serializationController.Serialize(uriHashes);
            await storageController.PushAsync(uriHashesFilename, serialiedData);
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
