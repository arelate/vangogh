using Models.Uris;

namespace Delegates.Values.Uri.ProductTypes
{
    public class GetApiProductsUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetApiProductsUpdateUriDelegate() :
            base(Uris.Endpoints.Api.ProductTemplate)
        {
            // ...
        }
    }
}