using System;
using System.Threading.Tasks;
using System.Net;

using Interfaces.Cookies;
using Interfaces.Storage;
using Interfaces.Serialization;

namespace Controllers.Cookies
{
    public class CookiesController : ICookiesController
    {
        private IStringStorageController storageController;
        private ISerializationController<string> serializationController;

        public CookiesController(
            IStringStorageController storageController,
            ISerializationController<string> serializationController)
        {
            this.storageController = storageController;
            this.serializationController = serializationController;
        }

        public async Task<Cookie[]> GetCookies()
        {
            var cookiesContent = await storageController.Pull("cookies.json");
            var cookies = serializationController.Deserialize<Cookie[]>(cookiesContent);

            if (cookies == null) return new Cookie[0];

            return cookies;
        }

        public async Task UpdateCookies(Cookie[] cookies)
        {
            throw new NotImplementedException();
        }
    }
}
