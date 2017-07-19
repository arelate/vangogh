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
        private IDictionary<string, string> storedCookies;
        private IGetFilenameDelegate getFilenameDelegate;

        public CookieController(
            ISerializedStorageController serializedStorageController,
            IGetFilenameDelegate getFilenameDelegate)
        {
            this.serializedStorageController = serializedStorageController;
            this.getFilenameDelegate = getFilenameDelegate;
            this.storedCookies = new Dictionary<string, string>();
        }

        public string GetCookiesString()
        {
            var cookieStringBuilder = new StringBuilder();
            foreach (var cookieNameValue in storedCookies)
                cookieStringBuilder.AppendFormat("{0}={1};", cookieNameValue.Key, cookieNameValue.Value);
            return cookieStringBuilder.ToString();
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
                var cookieNameValue = cookie.Substring(0, cookie.IndexOf(Separators.Common.SemiColon));
                var cookieNameValueParts = cookieNameValue.Split(
                    new string[] { Separators.Common.Equality }, 
                    StringSplitOptions.RemoveEmptyEntries);

                if (cookieNameValueParts.Length < 2) continue;

                var cookieName = cookieNameValueParts[0];
                var cookieValue = cookieNameValueParts[1];

                if (storedCookies.ContainsKey(cookieName))
                    storedCookies[cookieName] = cookieValue;
                else storedCookies.Add(cookieName, cookieValue);
            }

            await SaveAsync();
        }
    }
}
