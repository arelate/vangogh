using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

using Interfaces.Serialization;

using Models.Separators;

namespace Controllers.Serialization
{
    public class CookieSerializationController : IStrongTypeSerializationController<Cookie, string>
    {
        private const string expiresKey = "expires";
        private const string pathKey = "path";
        private const string domainKey = "domain";
        private const string httpOnlyKey = "HttpOnly";

        private const string keyValueTemplate = "{0}={1}";

        public Cookie Deserialize(string data)
        {
            var cookie = new Cookie();
            var firstKeyValueProcessed = false;

            var cookieParts = data.Split(new string[] { Separators.Common.SemiColon }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cookiePart in cookieParts)
            {
                var keyValuePair = cookiePart.Trim().Split(new string[] { Separators.KeyValue }, StringSplitOptions.RemoveEmptyEntries);
                var key = keyValuePair[0];
                var value = keyValuePair.Length > 1 ? keyValuePair[1] : string.Empty;

                if (!firstKeyValueProcessed)
                {
                    cookie.Name = key;
                    cookie.Value = value;
                    firstKeyValueProcessed = true;
                    continue;
                }

                switch (key)
                {
                    case expiresKey:
                        var expiresDateTime = DateTime.MaxValue;
                        if (DateTime.TryParse(value, out expiresDateTime))
                            cookie.Expires = expiresDateTime;
                        break;
                    case pathKey:
                        cookie.Path = value;
                        break;
                    case domainKey:
                        cookie.Domain = value;
                        break;
                    case httpOnlyKey:
                        cookie.HttpOnly = true;
                        break;
                    default: break;
                }
            }

            return cookie;
        }

        public string Serialize(Cookie cookie)
        {
            if (cookie == null) return string.Empty;

            var cookieParts = new List<string>();
            cookieParts.Add(string.Format(keyValueTemplate, cookie.Name, cookie.Value));
            cookieParts.Add(string.Format(keyValueTemplate, expiresKey, String.Format("{0:ddd,' 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' 'K}", cookie.Expires)));
            cookieParts.Add(string.Format(keyValueTemplate, pathKey, cookie.Path));
            cookieParts.Add(string.Format(keyValueTemplate, domainKey, cookie.Domain));
            if (cookie.HttpOnly) cookieParts.Add(httpOnlyKey);

            return string.Join(Separators.Common.Comma + Separators.Common.Space, cookieParts);
        }
    }
}
