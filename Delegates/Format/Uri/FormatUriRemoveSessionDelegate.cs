using System;

using Interfaces.Delegates.Format;

using Models.Separators;

namespace Delegates.Format.Uri
{
    public class FormatUriRemoveSessionDelegate : IFormatDelegate<string, string>
    {
        public string Format(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return string.Empty;

            var uriParts = uri.Split(new string[] { Separators.QueryString }, StringSplitOptions.RemoveEmptyEntries);

            if (uriParts == null) return string.Empty;
            if (uriParts.Length < 1) return string.Empty;

            return uriParts[0];
        }
    }
}
