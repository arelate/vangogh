using Models.Uris;

namespace Delegates.Format
{
    public class FormatImagesUriDelegate : FormatUriDelegate
    {
        public FormatImagesUriDelegate()
        {
            uriTemplate = Uris.Paths.Images.FullUriTemplate;
        }
    }
}
