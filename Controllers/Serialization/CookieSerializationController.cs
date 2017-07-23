using System;

using Interfaces.Serialization;

using Models.Separators;

namespace Controllers.Serialization
{
    public class CookieSerializationController : IStrongTypeSerializationController<(string, string), string>
    {
        private const string cookieNameValueTemplate = "{0}" + Separators.Common.Equality + "{1}";

        public (string, string) Deserialize(string cookie)
        {
            var deserializedCookie = new ValueTuple<string ,string>();

            var cookieNameValue = cookie.Substring(0, cookie.IndexOf(Separators.Common.SemiColon));
            var cookieNameValueParts = cookieNameValue.Split(
                new string[] { Separators.Common.Equality },
                StringSplitOptions.RemoveEmptyEntries);

            if (cookieNameValueParts.Length < 1) return deserializedCookie;
            deserializedCookie.Item1 = cookieNameValueParts[0];

            if (cookieNameValueParts.Length < 2) return deserializedCookie;
            deserializedCookie.Item2 = cookieNameValueParts[1];

            return deserializedCookie;
        }

        public string Serialize((string, string) cookie)
        {
            return string.Format(cookieNameValueTemplate, cookie.Item1, cookie.Item2);
        }
    }
}
