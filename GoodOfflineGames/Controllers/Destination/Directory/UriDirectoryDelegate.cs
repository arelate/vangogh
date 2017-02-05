using System;

using Interfaces.Destination.Directory;

using Models.Separators;

namespace Controllers.Destination.Directory
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
