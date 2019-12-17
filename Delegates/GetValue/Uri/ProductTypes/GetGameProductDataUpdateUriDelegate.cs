using Models.Uris;

namespace Delegates.GetValue.Uri.ProductTypes
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