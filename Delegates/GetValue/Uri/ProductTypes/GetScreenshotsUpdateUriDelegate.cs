using Models.Uris;

namespace Delegates.GetValue.Uri.ProductTypes
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