using System;
using System.IO;

using Interfaces.SourceDestination;

namespace Controllers.SourceDestination
{
    public class SourceUriDestinationController : ISourceDestinationController
    {
        public string GetSourceDestination(string sourceUri, string destination)
        {
            var uri = new Uri(sourceUri);
            return Path.Combine(destination, uri.Segments[uri.Segments.Length - 1]);
        }
    }
}
