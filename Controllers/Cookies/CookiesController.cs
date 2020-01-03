using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Cookies;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.StrongTypeSerialization;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Separators;

namespace Controllers.Cookies
{
    public class CookiesController : ICookiesController
    {
        readonly IStashController<Dictionary<string, string>> cookieStashController;
        readonly IStrongTypeSerializationController<(string, string), string> cookieSerializationController;
        IActionLogController actionLogController;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.Cookies.CookiesStashController,Controllers",
            "Controllers.StrongTypeSerialization.Cookies.CookiesSerializationController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public CookiesController(
            IStashController<Dictionary<string, string>> cookieStashController,
            IStrongTypeSerializationController<(string, string), string> cookieSerializationController,
            IActionLogController actionLogController)
        {
            this.cookieStashController = cookieStashController;
            this.cookieSerializationController = cookieSerializationController;
            this.actionLogController = actionLogController;
        }

        public async Task<string> GetCookiesStringAsync()
        {
            var storedCookies = await cookieStashController.GetDataAsync();

            var cookies = new List<string>();
            foreach (var cookieName in storedCookies.Keys)
            {
                var serializedCookie = cookieSerializationController.Serialize((cookieName, storedCookies[cookieName]));
                cookies.Add(serializedCookie);
            }
            return string.Join(Separators.Common.SemiColon, cookies);
        }

        public async Task SetCookiesAsync(IEnumerable<string> cookies)
        {
            var storedCookies = await cookieStashController.GetDataAsync();

            foreach (var cookie in cookies)
            {
                var deserializedCookie = cookieSerializationController.Deserialize(cookie);

                if (storedCookies.ContainsKey(deserializedCookie.Item1))
                    storedCookies[deserializedCookie.Item1] = deserializedCookie.Item2;
                else storedCookies.Add(deserializedCookie.Item1, deserializedCookie.Item2);
            }

            await cookieStashController.SaveAsync();
        }
    }
}
