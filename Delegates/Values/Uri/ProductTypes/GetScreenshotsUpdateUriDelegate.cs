using Models.Uris;

namespace Delegates.Values.Uri.ProductTypes
{
    public class GetScreenshotsUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetScreenshotsUpdateUriDelegate() :
            base(Uris.Endpoints.GameProductData.ProductTemplate)
        {
            // ...
        }
    }
}