namespace Delegates.Conversions.Uris
{
    public class ConvertScreenshotsUriTemplateToUriDelegate : ConvertUriTemplateToUriDelegate
    {
        public ConvertScreenshotsUriTemplateToUriDelegate()
        {
            uriTemplate = Models.Uris.Uris.Endpoints.Screenshots.FullUriTemplate;
        }
    }
}