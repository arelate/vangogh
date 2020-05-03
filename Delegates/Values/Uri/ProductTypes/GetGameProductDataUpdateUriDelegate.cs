using Models.Uris;

namespace Delegates.Values.Uri.ProductTypes
{
    public class GetGameProductDataUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetGameProductDataUpdateUriDelegate() :
            base(Uris.Endpoints.GameProductData.ProductTemplate)
        {
            // ...
        }
    }
}