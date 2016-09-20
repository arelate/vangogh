using System;

using Interfaces.Destination;

namespace Controllers.Destination.Abstract
{
    public abstract class ImagesScreenshotsDestinationController : IDestinationController
    {
        public virtual string GetDirectory(string source)
        {
            throw new NotImplementedException();
        }

        public virtual string GetFilename(string source)
        {
            var uri = new Uri(source);
            return uri.Segments[uri.Segments.Length - 1];
        }
    }
}
