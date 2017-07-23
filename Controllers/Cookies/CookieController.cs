using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
        private ISerializedStorageController serializedStorageController;
        private IStrongTypeSerializationController<(string, string), string> cookieSerializationController;
        private IDictionary<string, string> storedCookies;
        private IGetFilenameDelegate getFilenameDelegate;

        public CookieController(
            IStrongTypeSerializationController<(string, string), string> cookieSerializationController,
            ISerializedStorageController serializedStorageController,
            IGetFilenameDelegate getFilenameDelegate)
        {
            this.cookieSerializationController = cookieSerializationController;
            this.serializedStorageController = serializedStorageController;
            this.getFilenameDelegate = getFilenameDelegate;
            this.storedCookies = new Dictionary<string, string>();
        }

        public string GetCookiesString()
        {
            var cookies = new List<string>();
            foreach (var cookieName in storedCookies.Keys)
            {
                var serializedCookie = cookieSerializationController.Serialize((cookieName, storedCookies[cookieName]));
                cookies.Add(serializedCookie);
            }
            return string.Join(Separators.Common.SemiColon, cookies);
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
                var deserializedCookie = cookieSerializationController.Deserialize(cookie);

                if (storedCookies.ContainsKey(deserializedCookie.Item1))
                    storedCookies[deserializedCookie.Item1] = deserializedCookie.Item2;
                else storedCookies.Add(deserializedCookie.Item1, deserializedCookie.Item2);
            }

            await SaveAsync();
        }
    }
}
