using Models.Uris;

namespace Delegates.GetValue.Uri.ProductTypes
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