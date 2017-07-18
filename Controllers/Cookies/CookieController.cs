using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Interfaces.Cookies;
using Interfaces.Serialization;
using Interfaces.SerializedStorage;
using Interfaces.Destination.Filename;

using Models.Separators;

namespace Controllers.Cookies
{
    public class CookieController : ICookieController
    {
        //private IStrongTypeSerializationController<Cookie, string> cookieSerializationController;
        private ISerializedStorageController serializedStorageController;
        private IDictionary<string, string> storedCookies;
        private IGetFilenameDelegate getFilenameDelegate;

        public CookieController(
            //IStrongTypeSerializationController<Cookie, string> cookieSerializationController,
            ISerializedStorageController serializedStorageController,
            IGetFilenameDelegate getFilenameDelegate)
        {
            //this.cookieSerializationController = cookieSerializationController;
            this.serializedStorageController = serializedStorageController;
            this.getFilenameDelegate = getFilenameDelegate;
            this.storedCookies = new Dictionary<string, string>();
        }

        public IEnumerable<string> GetCookies()
        {
            foreach (var cookie in storedCookies.Values)
                yield return cookie;
        }

        public async Task LoadAsync()
        {
            storedCookies = await serializedStorageController.DeserializePullAsync<Dictionary<string, string>>(
                getFilenameDelegate.GetFilename());
            if (storedCookies == null)
                storedCookies = new Dictionary<string, string>();
        }

        public async Task SaveAsync()
        {
            await serializedStorageController.SerializePushAsync(
                getFilenameDelegate.GetFilename(),
                storedCookies);
        }

        public async Task SetCookies(IEnumerable<string> cookies)
        {
            foreach (var cookie in cookies)
            {
                var cookieName = cookie.Substring(0, cookie.IndexOf(Separators.Common.Equality));

                if (storedCookies.ContainsKey(cookieName))
                    storedCookies[cookieName] = cookie;
                else storedCookies.Add(cookieName, cookie);
            }

            await SaveAsync();
        }
    }
}
