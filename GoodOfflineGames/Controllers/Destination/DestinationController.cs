using System;
using System.IO;

using Interfaces.Destination;

namespace Controllers.Destination
{
    public class UriDestinationController : IDestinationController
    {
        public string GetDestination(string sourceUri, string destination)
        {
            var uri = new Uri(sourceUri);
            return Path.Combine(destination, uri.Segments[uri.Segments.Length - 1]);
        }
    }
}
