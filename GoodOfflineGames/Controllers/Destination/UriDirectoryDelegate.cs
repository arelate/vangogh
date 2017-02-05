using System;

using Interfaces.Destination;

using Models.Separators;

namespace Controllers.Destination
{
    public class UriDirectoryDelegate : IGetDirectoryDelegate
    {
        public string GetDirectory(string source = null)
        {
            var uriParts = source.Split(
                new string[] { Separators.UriPart }, 
                StringSplitOptions.RemoveEmptyEntries);

            return uriParts.Length >= 2 ?
                uriParts[uriParts.Length - 2] :
                source;
        }
    }
}
