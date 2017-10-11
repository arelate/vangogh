using Interfaces.ImageUri;

using Models.Uris;

namespace Controllers.ImageUri
{
    public class ExpandScreenshotUriDelegate : IExpandImageUriDelegate
    {
        public string ExpandImageUri(string partialUri)
        {
            return string.Format(Uris.Paths.Screenshots.FullUriTemplate, partialUri);
        }
    }
}
