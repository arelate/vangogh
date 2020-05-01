using Models.Uris;

namespace Delegates.Format.Uri
{
    public class FormatScreenshotsUriDelegate : FormatUriDelegate
    {
        public FormatScreenshotsUriDelegate()
        {
            uriTemplate = Uris.Endpoints.Screenshots.FullUriTemplate;
        }
    }
}