using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Cookies;
using Interfaces.Storage;
using Interfaces.Serialization;

namespace Controllers.Cookies
{
    public class CookiesController : ICookiesController
    {
        private Dictionary<string, string> cookies;
        private IStringStorageController storageController;
        private ISerializationController<string> serializationController;

        const char cookieSectionsSeparator = ';';
        const char cookiePropertyValueSeparator = '=';
        const string cookiesFilename = "cookies.json";

        public CookiesController(
            IStringStorageController storageController,
            ISerializationController<string> serializationController)
        {
            this.storageController = storageController;
            this.serializationController = serializationController;
        }

        public string GetName(string cookie)
        {
            if (string.IsNullOrEmpty(cookie)) return string.Empty;

            var firstIndexOfPropertyValueSeparator = cookie.IndexOf(cookiePropertyValueSeparator);
            return cookie.Substring(0, firstIndexOfPropertyValueSeparator);
        }

        public async Task<IEnumerable<string>> GetCookies()
        {
            if (cookies == null)
            {
                var cookiesContent = await storageController.Pull(cookiesFilename);
                cookies = serializationController.Deserialize<Dictionary<string, string>>(cookiesContent);
                if (cookies == null) cookies = new Dictionary<string, string>();
            }

            return cookies.Values;
        }

        public async Task UpdateCookies(IEnumerable<string> updatedCookies)
        {
            var somethingChanged = false;
            foreach (var cookie in updatedCookies)
            {
                var cookieName = GetName(cookie);

                if (!cookies.ContainsKey(cookieName))
                {
                    cookies.Add(cookieName, cookie);
                    somethingChanged = true;
                }
                else if (cookies[cookieName] != cookie)
                {
                    cookies[cookieName] = cookie;
                    somethingChanged = true;
                }
            }

            if (somethingChanged)
            {
                var cookiesContent = serializationController.Serialize(cookies);
                await storageController.Push(cookiesFilename, cookiesContent);
            }
        }
    }
}
