using Models.Uris;

namespace Delegates.Format.Uri
{
    public class FormatImagesUriDelegate : FormatUriDelegate
    {
        public FormatImagesUriDelegate()
        {
            uriTemplate = Uris.Paths.Images.FullUriTemplate;
        }
    }
}
