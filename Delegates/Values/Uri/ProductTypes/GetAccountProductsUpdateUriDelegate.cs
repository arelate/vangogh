using Models.Uris;

namespace Delegates.Values.Uri.ProductTypes
{
    public class GetAccountProductsUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetAccountProductsUpdateUriDelegate() :
            base(Uris.Endpoints.Account.GetFilteredProducts)
        {
            // ...
        }
    }
}