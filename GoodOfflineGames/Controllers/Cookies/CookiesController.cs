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
        private IStorageController<string> storageController;
        private ISerializationController<string> serializationController;

        const char cookieSectionsSeparator = ';';
        const char cookiePropertyValueSeparator = '=';
        const string cookiesFilename = "cookies.json";

        public CookiesController(
            IStorageController<string> storageController,
            ISerializationController<string> serializationController)
        {
            this.storageController = storageController;
            this.serializationController = serializationController;

            cookies = new Dictionary<string, string>();
        }

        public string GetName(string cookie)
        {
            if (string.IsNullOrEmpty(cookie)) return string.Empty;

            var firstIndexOfPropertyValueSeparator = cookie.IndexOf(cookiePropertyValueSeparator);
            return cookie.Substring(0, firstIndexOfPropertyValueSeparator);
        }

        public string GetValue(string cookie)
        {
            if (string.IsNullOrEmpty(cookie)) return string.Empty;

            var firstIndexOfPropertyValueSeparator = cookie.IndexOf(cookiePropertyValueSeparator);
            var firstIndexOfCookieSectionsSeparator = cookie.IndexOf(cookieSectionsSeparator);
            return cookie.Substring(firstIndexOfPropertyValueSeparator+1, firstIndexOfCookieSectionsSeparator - firstIndexOfPropertyValueSeparator-1);
        }

        public async Task<string> GetCookieHeader()
        {
            if (cookies.Count == 0)
            {
                var cookiesContent = await storageController.PullAsync(cookiesFilename);
                cookies = serializationController.Deserialize<Dictionary<string, string>>(cookiesContent);
                if (cookies == null) cookies = new Dictionary<string, string>();
            }

            var cookieHeaderBuilder = new System.Text.StringBuilder();

            foreach (var nameValue in cookies)
                cookieHeaderBuilder.Append($"{nameValue.Key}={nameValue.Value}");

            return cookieHeaderBuilder.ToString();
        }

        public async Task SetCookies(IEnumerable<string> setCookieHeaders)
        {
            if (setCookieHeaders == null) return;

            var somethingChanged = false;
            foreach (var setCookie in setCookieHeaders)
            {
                var cookieValue = GetValue(setCookie);
                var cookieName = GetName(setCookie);

                if (!cookies.ContainsKey(cookieName))
                {
                    cookies.Add(cookieName, cookieValue);
                    somethingChanged = true;
                }
                else if (cookies[cookieName] != cookieValue)
                {
                    cookies[cookieName] = cookieValue;
                    somethingChanged = true;
                }
            }

            if (somethingChanged)
            {
                var cookiesContent = serializationController.Serialize(cookies);
                await storageController.PushAsync(cookiesFilename, cookiesContent);
            }
        }
    }
}
