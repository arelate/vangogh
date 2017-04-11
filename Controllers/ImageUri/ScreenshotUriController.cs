using Interfaces.ImageUri;

using Models.Uris;

namespace Controllers.ImageUri
{
    public class ScreenshotUriController : IImageUriController
    {
        public string ExpandUri(string partialUri)
        {
            return string.Format(Uris.Paths.Screenshots.FullUriTemplate, partialUri);
        }
    }
}
