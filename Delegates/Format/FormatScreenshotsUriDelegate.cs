using Models.Uris;

namespace Delegates.Format
{
    public class FormatScreenshotsUriDelegate : FormatUriDelegate
    {
        public FormatScreenshotsUriDelegate()
        {
            uriTemplate = Uris.Paths.Screenshots.FullUriTemplate;
        }
    }
}
