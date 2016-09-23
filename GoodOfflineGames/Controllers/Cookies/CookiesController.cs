using System;
using System.Threading.Tasks;

using Interfaces.Cookies;

namespace Controllers.Cookies
{
    public class CookiesController : ICookiesController
    {
        public Task<string[]> GetCookies()
        {
            throw new NotImplementedException();
        }
    }
}
