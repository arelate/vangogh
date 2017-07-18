using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Cookies;
using Interfaces.Parsing;

namespace Controllers.Cookies
{
    public class CookieSerializationController : ICookieSerializationController
    {
        private IParsingController<IEnumerable<Cookie>> cookieParsingController;

        public CookieSerializationController(
            IParsingController<IEnumerable<Cookie>> cookieParsingController)
        {
            this.cookieParsingController = cookieParsingController;
        }

        public IEnumerable<Cookie> GetCookies()
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetCookies(string headers)
        {
            var cookies = cookieParsingController.Parse(headers);

            return null;
        }
    }
}
