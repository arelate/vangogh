using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Cookies;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.StrongTypeSerialization;

using Interfaces.Status;

using Models.Separators;

namespace Controllers.Cookies
{
    public class CookiesController : ICookiesController
    {
        readonly IStashController<Dictionary<string, string>> cookieStashController;
        readonly IStrongTypeSerializationController<(string, string), string> cookieSerializationController;
        IStatusController statusController;

        public CookiesController(
            IStashController<Dictionary<string, string>> cookieStashController,
            IStrongTypeSerializationController<(string, string), string> cookieSerializationController,
            IStatusController statusController)
        {
            this.cookieStashController = cookieStashController;
            this.cookieSerializationController = cookieSerializationController;
            this.statusController = statusController;
        }

        public async Task<string> GetCookiesStringAsync(IStatus status)
        {
            var storedCookies = await cookieStashController.GetDataAsync(status);

            var cookies = new List<string>();
            foreach (var cookieName in storedCookies.Keys)
            {
                var serializedCookie = cookieSerializationController.Serialize((cookieName, storedCookies[cookieName]));
                cookies.Add(serializedCookie);
            }
            return string.Join(Separators.Common.SemiColon, cookies);
        }

        public async Task SetCookiesAsync(IEnumerable<string> cookies, IStatus status)
        {
            var storedCookies = await cookieStashController.GetDataAsync(status);

            foreach (var cookie in cookies)
            {
                var deserializedCookie = cookieSerializationController.Deserialize(cookie);

                if (storedCookies.ContainsKey(deserializedCookie.Item1))
                    storedCookies[deserializedCookie.Item1] = deserializedCookie.Item2;
                else storedCookies.Add(deserializedCookie.Item1, deserializedCookie.Item2);
            }

            await cookieStashController.SaveAsync(status);
        }
    }
}
