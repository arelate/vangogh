using System;
using Interfaces.Delegates.Conversions;
using Models.Separators;

namespace Delegates.Conversions.Uris
{
    public class ConvertSessionUriToUriSansSessionDelegate : IConvertDelegate<string, string>
    {
        public string Convert(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return string.Empty;

            var uriParts = uri.Split(new string[] {Separators.QueryString}, StringSplitOptions.RemoveEmptyEntries);

            if (uriParts.Length < 1) return string.Empty;

            return uriParts[0];
        }
    }
}