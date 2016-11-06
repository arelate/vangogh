using System;
using System.Collections.Generic;

using Interfaces.Destination;

using Models.Separators;

namespace Controllers.Destination
{
    public class GOGUriDestinationController : IDestinationController
    {
        public string GetDirectory(string source)
        {
            var parts = GetUriParts(source);

            return parts.Length >= 2 ?
                parts[parts.Length - 2] :
                source;
        }

        public string GetFilename(string source)
        {
            var parts = GetUriParts(source);
            return RemoveQueryParameters(parts[parts.Length - 1]);
        }

        private string[] GetUriParts(string source)
        {
            return source.Split(new string[1] { Separators.UriPart }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string RemoveQueryParameters(string source)
        {
            var parts = source.Split(new string[1] { Separators.QueryString }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 1 ?
                parts[0] :
                source;
        }
    }
}
