using Interfaces.ImageUri;

using Models.Uris;

namespace Controllers.ImageUri
{
    public class ImageUriController : IImageUriController
    {
        public string ExpandUri(string partialUri)
        {
            return string.Format(Uris.Paths.Images.FullUriTemplate, partialUri);
        }
    }
}
