using Interfaces.ImageUri;

using Models.Uris;

namespace Controllers.ImageUri
{
    public class ExpandImageUriDelegate : IExpandImageUriDelegate
    {
        public string ExpandImageUri(string partialUri)
        {
            return string.Format(Uris.Paths.Images.FullUriTemplate, partialUri);
        }
    }
}
