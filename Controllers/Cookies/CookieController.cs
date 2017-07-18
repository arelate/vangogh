using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Interfaces.Cookies;
using Interfaces.Serialization;
using Interfaces.SerializedStorage;
using Interfaces.Destination.Filename;

namespace Controllers.Cookies
{
    public class CookieController : ICookieController
    {
        private IStrongTypeSerializationController<Cookie, string> cookieSerializationController;
        private ISerializedStorageController serializedStorageController;
        private IDictionary<string, Cookie> persistedCookies;
        private IGetFilenameDelegate getFilenameDelegate;

        public CookieController(
            IStrongTypeSerializationController<Cookie, string> cookieSerializationController,
            ISerializedStorageController serializedStorageController,
            IGetFilenameDelegate getFilenameDelegate)
        {
            this.cookieSerializationController = cookieSerializationController;
            this.serializedStorageController = serializedStorageController;
            this.getFilenameDelegate = getFilenameDelegate;
            this.persistedCookies = new Dictionary<string, Cookie>();
        }

        public IEnumerable<string> GetCookies()
        {
            foreach (var cookie in persistedCookies.Values)
                yield return cookieSerializationController.Serialize(cookie);
        }

        public async Task LoadAsync()
        {
            persistedCookies = await serializedStorageController.DeserializePullAsync<Dictionary<string, Cookie>>(
                getFilenameDelegate.GetFilename());
        }

        public async Task SaveAsync()
        {
            await serializedStorageController.SerializePushAsync(
                getFilenameDelegate.GetFilename(), 
                persistedCookies);
        }

        public async Task SetCookies(IEnumerable<string> cookies)
        {
            var parsedCookies = new List<Cookie>();
            foreach (var cookie in cookies)
                parsedCookies.Add(cookieSerializationController.Deserialize(cookie));

            // update persisted cookie - so that we store only one copy with the same name
            foreach (var cookie in parsedCookies)
            {
                if (persistedCookies.ContainsKey(cookie.Name))
                    persistedCookies[cookie.Name] = cookie;
                else persistedCookies.Add(cookie.Name, cookie);
            }

            await SaveAsync();
        }
    }
}
