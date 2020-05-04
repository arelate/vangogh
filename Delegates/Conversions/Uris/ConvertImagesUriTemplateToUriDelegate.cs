namespace Delegates.Conversions.Uris
{
    public class ConvertImagesUriTemplateToUriDelegate : ConvertUriTemplateToUriDelegate
    {
        public ConvertImagesUriTemplateToUriDelegate()
        {
            uriTemplate = Models.Uris.Uris.Endpoints.Images.FullUriTemplate;
        }
    }
}