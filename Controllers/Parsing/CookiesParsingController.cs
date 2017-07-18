using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

using Interfaces.Parsing;

namespace Controllers.Parsing
{
    public class CookieParsingController : IParsingController<IEnumerable<Cookie>>
    {
        public IEnumerable<Cookie> Parse(string input)
        {
            throw new NotImplementedException();
        }
    }
}
